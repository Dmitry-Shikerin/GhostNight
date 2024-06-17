//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using DungeonArchitect.Graphs;
using DungeonArchitect.MarkerGenerator;
using DungeonArchitect.Themeing;
using DungeonArchitect.SpatialConstraints;
using Random = UnityEngine.Random;

namespace DungeonArchitect
{
    /// <summary>
    /// The main dungeon behavior that manages the creation and destruction of dungeons
    /// </summary>
	[ExecuteInEditMode]
	public class Dungeon : MonoBehaviour {
        /// <summary>
        /// List of themes assigned to this dungeon
        /// </summary>
        public List<Graph> dungeonThemes;

        public MarkerGeneratorAsset[] patterns = Array.Empty<MarkerGeneratorAsset>();
	        
        /// <summary>
        /// Draw debug data
        /// </summary>
        public bool debugDraw = false;

        DungeonConfig config;
        PooledDungeonSceneProvider sceneProvider;
        DungeonBuilder dungeonBuilder;
        DungeonModel dungeonModel;
        DungeonQuery dungeonQuery;
        DungeonSceneObjectSpawner objectSpawner;

        /// <summary>
        /// Active model used by the dungeon
        /// </summary>
		public DungeonModel ActiveModel {
			get {
				if (dungeonModel == null) {
					dungeonModel = GetComponent<DungeonModel> ();
				}
				return dungeonModel;
			}
		}

        /// <summary>
        /// Flag to check if the layout has been built.  
        /// This is used to quickly reapply the theme after the theme graph has been modified,
        /// without rebuilding the layout, if it has already been built
        /// </summary>
		public bool IsLayoutBuilt {
			get {
                if (dungeonBuilder == null)
                {
                    return false;
                }
                return dungeonBuilder.IsLayoutBuilt;
			}
		}

        //[SerializeField]
        LevelMarkerList markers = new LevelMarkerList();
        public LevelMarkerList Markers
        {
            get { return markers; }
        }


        /// <summary>
        /// Flag to rebuild the dungeon. Set this to true if you want to rebuild it in the next update
        /// </summary>
		bool requestedRebuild = false;

		public DungeonConfig Config {
			get {
				if (config == null) {
					config = GetComponent<DungeonConfig> ();
				}
				return config;
			}
		}

        void Awake() {
            Initialize();
		}
        

		void Initialize() {
			if (config == null) {
				config = GetComponent<DungeonConfig> ();
			}
			
			if (sceneProvider == null) {
				sceneProvider = GetComponent<PooledDungeonSceneProvider> ();
			}
			
			if (dungeonBuilder == null) {
				dungeonBuilder = GetComponent<DungeonBuilder> ();
			}

			if (dungeonQuery == null)
			{
				dungeonQuery = GetComponent<DungeonQuery>();
			}

			if (dungeonModel == null) {
				dungeonModel = GetComponent<DungeonModel> ();
			}
		}

        public List<DungeonThemeData> GetThemeAssets()
        {
            var themes = new List<DungeonThemeData>();
            foreach (var themeGraph in dungeonThemes)
            {
                DungeonThemeData theme = new DungeonThemeData();
                theme.BuildFromGraph(themeGraph);
                themes.Add(theme);
            }
            return themes;
        }

        
        /// <summary>
        /// Builds the complete dungeon (layout and visual phase)
        /// </summary>
		public void Build() {
            Build(new RuntimeDungeonSceneObjectInstantiator());
        }


        /// <summary>
        /// Set the seed of the dungeon.  The seed determines the layout of the dungeon.
        /// Change this number to get a different layout.
        /// Use the same seed to get the same dungeon  
        /// </summary>
        /// <param name="seed"></param>
        public void SetSeed(int seed)
        {
	        Config.Seed = (uint) seed;
        }
        
        /// <summary>
        /// Randomizes the seed to generate a new dungeon layout
        /// </summary>
        public void RandomizeSeed()
        {
	        SetSeed(Mathf.RoundToInt(Random.value * int.MaxValue));
        }
        
        /// <summary>
        /// Randomizes the seed to generate a new dungeon layout
        /// </summary>
        public void RandomizeSeed(System.Random randomStream)
        {
	        SetSeed(Mathf.RoundToInt(randomStream.NextFloat() * int.MaxValue));
        }
        
        public void Build(IDungeonSceneObjectInstantiator objectInstantiator)
        {
	        if (dungeonBuilder.DestroyDungeonOnRebuild())
	        {
		        DestroyDungeon();
	        }
	        
            NotifyPreBuild();

            Initialize();
			dungeonModel.ResetModel();

			dungeonBuilder.BuildDungeon(config, dungeonModel);
            markers = dungeonBuilder.Markers;

            if (dungeonQuery != null)
            {
	            dungeonQuery.OnPostLayoutBuild();
            }
            
			NotifyPostLayoutBuild();

			// Spawn the scene objects
			dungeonBuilder.SpawnManagedObjects(sceneProvider, objectInstantiator);
            if (dungeonBuilder.IsThemingSupported())
            {
                ApplyTheme(objectInstantiator);
            }

            if (dungeonQuery != null)
            {
	            dungeonQuery.OnPostDungeonBuild();
            }

            // Build the navigation
            var navigation = GetComponent<DungeonRuntimeNavigation>();
            if (navigation != null)
            {
                navigation.BuildNavMesh();
            }

            NotifyPostBuild();
        }

        private void ApplyPatternMatchers()
        {
	        if (patterns == null || dungeonBuilder == null || dungeonBuilder.Markers == null)
	        {
		        return;
	        }

	        var random = new System.Random(config != null ? (int)config.Seed : 0);
	        bool processed = false;
	        var markerList = dungeonBuilder.Markers.GetMarkerList();
	        foreach (var patternAsset in patterns)
	        {
		        foreach (var pattern in patternAsset.patterns)
		        {
			        var processor = dungeonBuilder.CreateMarkerGenProcessor();
			        if (processor == null)
			        {
				        continue;
			        }

			        if (processor.Process(pattern, markerList, random, out var newMarkerList))
			        {
				        markerList = newMarkerList;
				        processed = true;
			        }

			        processor.Release();
		        }
	        }

	        if (processed)
	        {
		        dungeonBuilder.Markers.Set(markerList);
	        }
        }

        private void EmitMarkers()
        {
	        if (dungeonBuilder == null || !dungeonBuilder.IsThemingSupported())
	        {
		        return;
	        }
	        
	        // Emit markers defined by this builder
	        dungeonBuilder.EmitMarkers();

	        // Emit markers defined by the users (by attaching implementation of DungeonMarkerEmitter behaviors)
	        dungeonBuilder.EmitCustomMarkers();

	        ApplyPatternMatchers();
            
	        NotifyMarkersEmitted(dungeonBuilder.Markers);
        }
        
        /// <summary>
        /// Runs the theming engine over the existing layout to rebuild the game objects from the theme file.  
        /// The layout is not built in this stage
        /// </summary>
        public void ApplyTheme(IDungeonSceneObjectInstantiator objectInstantiator) {
	        if (dungeonBuilder == null)
	        {
		        return;
	        }
	        
	        EmitMarkers();
	        
            var themes = GetThemeAssets();
            var themeContext = CreateThemeExecutionContext(objectInstantiator);
            var themeEngine = new DungeonThemeEngine(themeContext);
            themeEngine.ApplyTheme(dungeonBuilder.Markers, themes);
        }

        DungeonThemeExecutionContext CreateThemeExecutionContext(IDungeonSceneObjectInstantiator objectInstantiator)
        {
            var context = new DungeonThemeExecutionContext();
            context.builder = dungeonBuilder;
            context.config = config;
            context.model = dungeonModel;
            context.spatialConstraintProcessor = GetComponent<SpatialConstraintProcessor>();
            context.sceneProvider = GetComponent<DungeonSceneProvider>();
            context.objectInstantiator = objectInstantiator;
            context.spawnListeners = GetComponents<DungeonItemSpawnListener>().ToArray();

            var builder = GetComponent<DungeonBuilder>();
            if (builder.asyncBuild)
            {
                var buildPosition = (builder.asyncBuildStartPosition != null) ? builder.asyncBuildStartPosition.position : Vector3.zero;
                objectSpawner = new AsyncDungeonSceneObjectSpawner(builder.maxBuildTimePerFrame, buildPosition);
            }
            else
            {
                objectSpawner = new SyncDungeonSceneObjectSpawner();
            }

            context.objectSpawner = objectSpawner;

            var themeOverrides = new List<ThemeOverrideVolume>();
            var dungeon = GetComponent<Dungeon>();

            // Process the theme override volumes
            var volumes = GameObject.FindObjectsOfType<ThemeOverrideVolume>();
            foreach (var volume in volumes)
            {
                if (volume.dungeon != dungeon)
                {
                    continue;
                }
                themeOverrides.Add(volume);
            }
            context.themeOverrideVolumes = themeOverrides.ToArray();

            return context;
        }

        DungeonEventListener[] GetListeners() {
			var listeners = GetComponents<DungeonEventListener>();

			var enabledListeners = from listener in listeners
					where listener.enabled
					select listener;

			return enabledListeners.ToArray();
		}

		void NotifyPostLayoutBuild() {
			// Notify all listeners of the post build event
			foreach (var listener in GetListeners()) {
				listener.OnPostDungeonLayoutBuild(this, ActiveModel);
			}
		}

        void NotifyPreBuild()
        {
            // Notify all listeners of the post build event
            foreach (var listener in GetListeners())
            {
                listener.OnPreDungeonBuild(this, ActiveModel);
            }
        }

        void NotifyPostBuild() {
			// Notify all listeners of the post build event
			foreach (var listener in GetListeners()) {
				listener.OnPostDungeonBuild(this, ActiveModel);
			}
		}

        void NotifyMarkersEmitted(LevelMarkerList markers)
        {
            // Notify all listeners of the post build event
            foreach (var listener in GetListeners())
            {
                if (listener == null) continue;
                listener.OnDungeonMarkersEmitted(this, ActiveModel, markers);
            }
        }

        void NotifyPreDungeonDestroy()
        {
            // Notify all listeners that the dungeon is destroyed
            foreach (var listener in GetListeners())
            {
                listener.OnPreDungeonDestroy(this);
            }
        }

        void NotifyDungeonDestroyed() {
			// Notify all listeners that the dungeon is destroyed
			foreach (var listener in GetListeners()) {
				listener.OnDungeonDestroyed(this);
			}
		}

        /// <summary>
        /// Destroys the dungeon
        /// </summary>
		public void DestroyDungeon() {
            NotifyPreDungeonDestroy();
            
            var itemList = GameObject.FindObjectsOfType<DungeonSceneProviderData>();
            var dungeonItems = new List<GameObject>();
            foreach (var item in itemList)
            {
                if (item == null) continue;
                if (item.dungeon == this)
                {
                    dungeonItems.Add(item.gameObject);
                }
            }
			foreach(var item in dungeonItems) {
				if (Application.isPlaying) {
					Destroy(item);
				} else {
					DestroyImmediate(item);
				}
			}
            
            if (objectSpawner != null)
            {
                objectSpawner.Destroy();
                objectSpawner = null;
            }

            // Build the navigation
            var navigation = GetComponent<DungeonRuntimeNavigation>();
            if (navigation != null) {
                navigation.DestroyNavMesh();
            }

            if (dungeonQuery != null)
            {
	            dungeonQuery.Release();
            }
            
            if (dungeonModel != null) {
				dungeonModel.ResetModel();
			}

			if (dungeonBuilder != null) {
				dungeonBuilder.OnDestroyed();
			}

			NotifyDungeonDestroyed();
		}

		/// <summary>
		/// Requests the dungeon to be rebuilt in the next update phase
		/// </summary>
		public void RequestRebuild() {
			requestedRebuild = true;
		}

		public virtual void Update() {
			if (dungeonModel == null) return;
			
			if (requestedRebuild) {
				requestedRebuild = false;
				Build();
            }
            if (debugDraw)
            {
                DebugDraw();
            }

            if (objectSpawner != null)
            {
                objectSpawner.Tick();
            }
        }

		void OnGUI()
        {
        }

		void DebugDraw() {
            if (dungeonBuilder != null)
            {
                dungeonBuilder.DebugDraw();
            }
        }

        void OnDrawGizmosSelected()
        {
            if (debugDraw && dungeonBuilder != null)
            {
                dungeonBuilder.DebugDrawGizmos();
            }
        }

	}
	
}
