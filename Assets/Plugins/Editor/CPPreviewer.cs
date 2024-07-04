using UnityEngine;
using System.Collections;
using UnityEditor;

public class CPPreviewer : EditorWindow
{
    
    float animNormal = 0f;

    float prevLength=1f;
    float slider2 = 1f;

    float prevSpeed = 0.001f;
	
	bool syncFx=false;

    Vector2 scr1;


    public Object character;
    public Object particleParent;

    GameObject charG;
    GameObject parG;

    bool showProgress = true;
    bool showSpec = false;

    bool playing = false;

    int currentID = 0;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Tools/AnimationPreviwer")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        CPPreviewer window = (CPPreviewer)EditorWindow.GetWindow(typeof(CPPreviewer));
    }


    void Update()
    {
        Repaint();
    }

    void OnGUI()
    {
        GUILayout.Label("Animation/FX Previewer", EditorStyles.boldLabel);

        GUILayout.Space(20);

        GUILayout.Label("Drag GameObject(with animation component on it) to field below from hierarchy");

        //EditorGUILayout.ObjectField(
        //myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);

        character = EditorGUILayout.ObjectField("Character Obj:", character,typeof(GameObject));
		
		
		syncFx=EditorGUILayout.BeginToggleGroup("Sync ParticleFx",syncFx);
        //GUILayout.BeginHorizontal();
        particleParent = EditorGUILayout.ObjectField("ParticeSystem Obj:", particleParent, typeof(GameObject));
		
		EditorGUILayout.EndToggleGroup();

        //GUILayout.EndHorizontal();
        
        if (character != null)
        {
            charG = character as GameObject;

            if (charG.GetComponent<Animation>() == null)
            {
                GUI.color = Color.yellow;
                GUILayout.Label("[Error:missing animation component!]");
                GUI.color = Color.white;
            }
            else
            {
                GUILayout.Space(10);


                scr1 = GUILayout.BeginScrollView(scr1,GUILayout.Height(300));

                int i = 0;
                foreach (AnimationClip ani in AnimationUtility.GetAnimationClips(charG.GetComponent<Animation>()))
                {
                    if (currentID == i)
                    { GUI.color = Color.yellow; }
                    
                    if (GUILayout.Button(ani.name,GUILayout.Height(25)))
                    {

                        charG.GetComponent<Animation>().clip = ani;

                        currentID = i;
                    }

                    GUI.color = Color.white;

                    i++;
                }

                GUILayout.EndScrollView();

                GUILayout.Space(15);

                showProgress = !showSpec;

                showProgress = EditorGUILayout.BeginToggleGroup("Preview by anim length", showProgress);
                
                //GUILayout.BeginHorizontal();
                animNormal = EditorGUILayout.Slider("Current Progress:("+(animNormal*100).ToString("0")+"%)", animNormal, 0, 1f);
                GUILayout.Label("Current Time:" + (charG.GetComponent<Animation>()[charG.GetComponent<Animation>().clip.name].length * animNormal).ToString("0.000") + "(s)");


                EditorGUILayout.EndToggleGroup();

                EditorGUILayout.Separator();

                showSpec = !showProgress;

                showSpec = EditorGUILayout.BeginToggleGroup("Preview by specific length", showSpec);


                prevLength = EditorGUILayout.FloatField("Set Length:(s)", prevLength);
                slider2 = EditorGUILayout.Slider("Current Time(s):", slider2, 0, prevLength);

                EditorGUILayout.EndToggleGroup();


                EditorGUILayout.Separator();

                if (!playing)
                {
                    if (GUILayout.Button("Play Animation",GUILayout.Height(25)))
                    {
                        playing = true;
                    }


                    
                }
                else
                {
					GUI.color=Color.yellow;
                    if (GUILayout.Button("Stop", GUILayout.Height(25)))
                    {
                        playing = false;
                    }

                    animNormal += prevSpeed;
                    if (animNormal >= 1) { animNormal = 0; }

                    slider2 += prevSpeed;
                    if (slider2 >= prevLength) { slider2 = 0; }

                }
				
				GUI.color=Color.white;

                prevSpeed = EditorGUILayout.Slider("Anim Speed:", prevSpeed, 0, 0.016f);


                AnimationState state = charG.GetComponent<Animation>()[charG.GetComponent<Animation>().clip.name];

                if (!charG.GetComponent<Animation>().IsPlaying(charG.GetComponent<Animation>().clip.name))
                {
                    charG.GetComponent<Animation>().wrapMode = WrapMode.Loop;
                    charG.GetComponent<Animation>().Play(charG.GetComponent<Animation>().clip.name);
                }

                if (showProgress)
                {
                    state.normalizedTime = animNormal;
                }
                else
                {
                    state.time = slider2;
                }

                state.enabled = true;
                charG.GetComponent<Animation>().Sample();
                state.enabled = false;

               
            }
        }


        if (particleParent != null&&syncFx)
        {
            parG = particleParent as GameObject;

            if (parG != null)
            {
                ParticleSystem[] ps = parG.GetComponentsInChildren<ParticleSystem>();

                foreach (ParticleSystem p in ps)
                {
                    if (showProgress)
                    {
                        p.Simulate(animNormal);
                    }
                    else
                    {
                        p.Simulate(slider2);
                    }
                }
            }
        }
        
    }

    void OnDisable()
    {
		if(charG!=null&&charG.GetComponent<Animation>()!=null)
		{
         AnimationState state = charG.GetComponent<Animation>()[charG.GetComponent<Animation>().clip.name];

         state.normalizedTime = 0;
         state.enabled = true;
         charG.GetComponent<Animation>().Sample();
         state.enabled = false;
		}
    }
}