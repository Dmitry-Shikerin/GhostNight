//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DungeonArchitect.SxEngine
{
    public enum SxTextHAlign
    {
        Left,
        Center,
        Right
    }

    public enum SxTextVAlign
    {
        Top,
        Center,
        Bottom
    }

    public struct SxTextComponentSettings
    {
        public Font Font;
        public Color Color;
        public float Scale;
        public SxTextHAlign HAlign;
        public SxTextVAlign VAlign;
        public float DepthBias;
        public SxMaterial CustomMaterial;
        public bool WordWrap;
        public float WordWrapWidth;
        public float WordWrapHeight;
    }

    public struct SxTextLineInfo
    {
        public string Text;
        public float Scale;
        public float PaddingTop;
        public float PaddingBottom;
    }

    public class SxTextLineList
    {
        private List<SxTextLineInfo> lines = new List<SxTextLineInfo>();
        public SxTextLineInfo[] Lines => lines.ToArray();
        public int LineSpacing = 10;

        public int Length => lines.Count;
        
        public SxTextLineList()
        {
        }

        public SxTextLineList(string message)
        {
            AddLine(message);
        }

        public void Clear()
        {
            lines.Clear();
        }

        public void AddLine(string message, float scale = 1, float paddingTop = 0, float paddingBottom = 0)
        {   
            lines.Add(new SxTextLineInfo()
            {
                Text = message, 
                Scale = scale,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom
            });
        }
    }
    
    public class SxTextComponent : SxActorComponent
    {
        private SxMesh mesh;
        private SxMaterial material;
        private SxTextLineList lines = new SxTextLineList();
        public SxTextComponentSettings Settings = new SxTextComponentSettings();

        public void SetText(string message)
        {
            lines = new SxTextLineList(message);
            RebuildMesh();
        }

        public void SetLines(SxTextLineList newLines)
        {
            lines = newLines;
            RebuildMesh();
        }

        private const string ValidCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890_`~!@#$%^&*()-+=[]{}\\|;:'\"<>,./? ";

        public void Initialize(SxTextComponentSettings textSettings)
        {
            this.Settings = textSettings;
            Font.textureRebuilt += OnTextureRebuilt;

            if (Settings.Font != null)
            {
                Settings.Font.RequestCharactersInTexture(ValidCharacters);
                UpdateMaterial(Settings.Font);
                RebuildMesh();
            }
        }

        void UpdateMaterial(Font font)
        {
            if (font != null && font.material != null)
            {
                if (Settings.CustomMaterial != null)
                {
                    material = Settings.CustomMaterial;
                    material.UnityMaterial.mainTexture = font.material.mainTexture;
                }
                else
                {
                    material = new SxUnityMaterial(font.material);
                    material.DepthBias = Settings.DepthBias;
                }
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            Font.textureRebuilt -= OnTextureRebuilt;
        }

        private void OnTextureRebuilt(Font font)
        {
            if (font == Settings.Font)
            {
                UpdateMaterial(font);
                RebuildMesh();
            }
        }

        protected override void DrawImpl(SxRenderContext context, Matrix4x4 accumWorldTransform, SxRenderCommandList renderCommandList)
        {
            if (mesh != null && material != null)
            {
                renderCommandList.Add(new SxRenderCommand(accumWorldTransform, mesh, material));
            }
        }

        private static void CalcTextSize(string message, float scale, Font font, out float width, out float minY, out float maxY)
        {
            width = 0;
            minY = maxY = 0;
            
            var localScale = scale / font.fontSize;
            for (var i = 0; i < message.Length; i++)
            {
                CharacterInfo ch;
                font.GetCharacterInfo(message[i], out ch);
                width += ch.advance * localScale;
                minY = Mathf.Min(minY, ch.minY * localScale);
                maxY = Mathf.Max(maxY, ch.maxY * localScale);
            }
        }

        private float CalculateTextHeight(SxTextLineList lines, float scale)
        {
            float height = 0;
            for (var i = 0; i < lines.Lines.Length; i++)
            {
                var line = lines.Lines[i];
                var lineScale = scale * line.Scale;
                CalcTextSize(line.Text, lineScale, Settings.Font, out var _, out var minY, out var maxY);
                height += maxY - minY;
                
                var localScale = scale * line.Scale / Settings.Font.fontSize;
                if (i > 0)
                {
                    height += lines.LineSpacing * localScale;
                }

                height += (line.PaddingTop + line.PaddingBottom) * localScale;
            }

            return height;
        }

        private static SxTextLineList PerformWordWrap(SxTextLineList lineList, float maxWidth, float scale, Font font)
        {
            var result = new SxTextLineList();
            foreach (var lineInfo in lineList.Lines)
            {
                var baseScale = scale * lineInfo.Scale;
                string[] wrappedLines = PerformWordWrap(lineInfo.Text, maxWidth, baseScale, font);
                for (int i = 0; i < wrappedLines.Length; i++)
                {
                    result.AddLine(wrappedLines[i], lineInfo.Scale, 
                        (i == 0) ? lineInfo.PaddingTop : 0,
                        (i == wrappedLines.Length - 1 ? lineInfo.PaddingBottom : 0));
                }
            }

            return result;
        }
        
        class TokenInfo
        {
            public string Message;
            public float Width = 0;
            public float MinY = 0;
            public float MaxY = 0;
        }
        
        private static string[] PerformWordWrap(string message, float maxWidth, float scale, Font font)
        {
            var textTokens = message.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var tokens = new TokenInfo[textTokens.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                tokens[i] = new TokenInfo
                {
                    Message = textTokens[i]
                };
                CalcTextSize(textTokens[i], scale, font, out tokens[i].Width, out tokens[i].MinY, out tokens[i].MaxY);
            }

            CalcTextSize(" ", scale, font, out var spaceWidth, out var _, out var _);

            var lines = new List<string>();
            var line = new StringBuilder();
            var lineWidth = 0.0f;
            for (int i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                var tokenWidth = tokens[i].Width;
                if (line.Length > 0 && lineWidth + spaceWidth + tokenWidth > maxWidth)
                {
                    lines.Add(line.ToString());
                    line.Clear();
                    lineWidth = 0;
                }

                if (line.Length > 0)
                {
                    line.Append(" ");
                    lineWidth += spaceWidth;
                }

                line.Append(token.Message);
                lineWidth += tokenWidth;
            }

            if (line.Length > 0)
            {
                lines.Add(line.ToString());
            }

            return lines.ToArray();
        }

        private void RebuildMesh()
        {
            mesh = null;
            if (material == null || lines.Length == 0 || Settings.Font == null)
            {
                return;
            }

            UpdateMaterial(Settings.Font);

            var textWidth = 0.0f;
            var textHeight = 0.0f;
            var pos = Vector3.zero;
            var vertices = new List<SxMeshVertex>();

            var activeScale = Settings.Scale;
            var wrappedLines = lines;
            if (Settings.WordWrap)
            {
                wrappedLines = PerformWordWrap(lines, Settings.WordWrapWidth, activeScale, Settings.Font);
                if (Settings.WordWrapHeight > 1e-4f)
                {
                    var worldHeight = CalculateTextHeight(wrappedLines, activeScale);
                    if (worldHeight > Settings.WordWrapHeight)
                    {
                        var scaleLow = 0.0f;
                        var scaleHigh = 1.0f;
                        const int maxTries = 10;
                        int numTries = 0;
                        float bestScale = activeScale;
                        float bestHeight = worldHeight;
                        
                        while (numTries < maxTries)
                        {
                            var mid = (scaleLow + scaleHigh) * 0.5f;
                            var midScale = activeScale * mid;
                            wrappedLines = PerformWordWrap(lines, Settings.WordWrapWidth, midScale, Settings.Font);
                            worldHeight = CalculateTextHeight(wrappedLines, midScale);
                            if (worldHeight < Settings.WordWrapHeight)
                            {
                                scaleLow = mid;
                            }
                            else
                            {
                                scaleHigh = mid;
                            }

                            if (bestHeight > Settings.WordWrapHeight)
                            {
                                if (bestHeight > worldHeight)
                                {
                                    bestHeight = worldHeight;
                                    bestScale = midScale;
                                }
                            }
                            else
                            {
                                if (bestHeight < worldHeight)
                                {
                                    bestHeight = worldHeight;
                                    bestScale = midScale;
                                }
                            }
                            numTries++;
                        }

                        activeScale = bestScale;
                    }
                }
            }
            
            foreach (var lineInfo in wrappedLines.Lines.Reverse())
            {
                var baseScale = activeScale * lineInfo.Scale;
                var scale = baseScale / Settings.Font.fontSize;
                pos.y += lineInfo.PaddingBottom * scale;
                
                var line = lineInfo.Text;

                var lineHeight = 0.0f;
                var lineWidth = 0.0f;
                for (var i = 0; i < line.Length; i++)
                {
                    CharacterInfo ch;
                    Settings.Font.GetCharacterInfo(line[i], out ch);
                    var p0 = pos + new Vector3(ch.minX, ch.maxY, 0) * scale;
                    var p1 = pos + new Vector3(ch.maxX, ch.maxY, 0) * scale;
                    var p2 = pos + new Vector3(ch.maxX, ch.minY, 0) * scale;
                    var p3 = pos + new Vector3(ch.minX, ch.minY, 0) * scale;

                    var t0 = ch.uvTopLeft;
                    var t1 = ch.uvTopRight;
                    var t2 = ch.uvBottomRight;
                    var t3 = ch.uvBottomLeft;

                    vertices.Add(new SxMeshVertex(p0, Settings.Color, t0));
                    vertices.Add(new SxMeshVertex(p1, Settings.Color, t1));
                    vertices.Add(new SxMeshVertex(p2, Settings.Color, t2));
                    vertices.Add(new SxMeshVertex(p3, Settings.Color, t3));

                    pos += new Vector3(ch.advance * scale, 0, 0);

                    lineHeight = Mathf.Max(lineHeight, ch.maxY * scale);
                    lineWidth += ch.advance * scale;
                }

                textWidth = Mathf.Max(textWidth, lineWidth);
                var paddedHeight = lineHeight + lines.LineSpacing * scale;
                textHeight += paddedHeight;

                pos.x = 0;
                pos.y += paddedHeight;
                
                pos.y += lineInfo.PaddingTop * scale;
            }
            
            var offset = Vector3.zero;
            if (Settings.VAlign == SxTextVAlign.Center)
            {
                offset.y -= textHeight * 0.5f;
            }
            else if (Settings.VAlign == SxTextVAlign.Top)
            {
                offset.y -= textHeight;
            }

            if (Settings.HAlign == SxTextHAlign.Center)
            {
                offset.x -= textWidth * 0.5f;
            }
            else if (Settings.HAlign == SxTextHAlign.Right)
            {
                offset.x -= textWidth;
            }

            foreach (var vertex in vertices)
            {
                vertex.Position += offset;
            }

            mesh = new SxMesh();
            mesh.CreateSection(0, GL.QUADS, vertices.ToArray());
        }

    }
}