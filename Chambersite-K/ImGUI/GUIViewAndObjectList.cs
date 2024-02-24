using Assimp.Unmanaged;
using Chambersite_K.GameObjects;
using Chambersite_K.Graphics;
using Chambersite_K.Views;
using Chambersite_K.World;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.ImGUI
{
    internal class GUIViewAndObjectList : ImGuiLayout
    {
        public GUIViewAndObjectList() { SetBaseBehaviour(true); }

        public void Draw()
        {
            if (StartLayout("Game Inspector", new System.Numerics.Vector2(600, 600)))
            {
                if (ImGui.BeginTabBar("##objectInspector"))
                {
                    if (ImGui.BeginTabItem("Views"))
                    {
                        foreach (IView view in GAME.ActiveViews)
                        {
                            if (ImGui.CollapsingHeader(view.ToString()))
                            {
                                DisplayView(view);
                            }
                        }
                        ImGui.EndTabItem();
                    }
                    if (ImGui.BeginTabItem("GameObjects"))
                    {
                        List<Tuple<string, GameObjectPool>> goHolders = new List<Tuple<string, GameObjectPool>>
                    { new Tuple<string, GameObjectPool>("Global GameObjects", GAME.ObjectPool) };
                        foreach (IView view in GAME.ActiveViews)
                            goHolders.Add(new Tuple<string, GameObjectPool>(view.ToString(), view.ObjectPool));

                        foreach (Tuple<string, GameObjectPool> container in goHolders)
                        {
                            if (ImGui.CollapsingHeader($"{container.Item1}"))
                            {
                                DisplayGameObject(container.Item2);
                            }
                        }
                        ImGui.EndTabItem();
                    }
                    if (ImGui.BeginTabItem("Resources"))
                    {
                        List<Tuple<string, List<IResource>>> resourceHolders = new List<Tuple<string, List<IResource>>>
                    { new Tuple<string, List<IResource>>("Global Pool", GAME.ResourcePool) };
                        foreach (IView view in GAME.ActiveViews)
                            resourceHolders.Add(new Tuple<string, List<IResource>>(view.ToString(), view.ResourcePool));

                        foreach (Tuple<string, List<IResource>> container in resourceHolders)
                        {
                            if (ImGui.CollapsingHeader($"{container.Item1}"))
                            {
                                DisplayResourcePool(container.Item2);
                            }
                        }
                        ImGui.EndTabItem();
                    }
                    ImGui.EndTabBar();
                }
                EndLayout();
            }
        }

        private void DisplayView(IView view)
        {
            ImGui.Text($"Type: {view.GetType().Name}");
            ImGui.Text($"View Type: {view.ViewType}");
            ImGui.Text($"Status: {view.ViewStatus}");
            ImGui.Text($"Timer: {view.Timer}");
            PropertyInfo[] properties = view.GetType().GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                if (prop.PropertyType == typeof(World3D))
                {
                    DisplayWorld3D((World3D)prop.GetValue(view));
                }
            }
            if (ImGui.TreeNode($"More details for {view.InternalName}..."))
            {
                foreach (var property in properties)
                {
                    var value = property.GetValue(view, null);
                    if (value == null) value = "null";
                    ImGui.Text($"{property.Name} ({value.GetType().Name}) = {value}");
                }
                ImGui.TreePop();
            }
        }

        private void DisplayGameObject(GameObjectPool goPool)
        {
            ImGui.Text($"Total GameObject count: {goPool.GetAllObjectCount()}");
            if (goPool.GetAllObjectCount() != 0)
            {
                ImGui.Text($"Children: ({goPool.ObjectPool.Count})");
                DisplayViewChildren(goPool.ObjectPool, 20);
            }
        }

        /// <summary>
        /// Will recursively display all of a GameObject Children.<br/>
        /// To avoid possible infinite calls, this functions has a max call depth of 20.
        /// </summary>
        /// <param name="obj"></param>
        private void DisplayViewChildren(List<GameObject> Children, int _CallDepth)
        {
            if (_CallDepth <= 0)
                return;
            foreach (GameObject child in Children)
            {
                if (ImGui.TreeNode(child.ToString()))
                {
                    ImGui.Text($"Status: {child.Status}");
                    ImGui.Text($"Position: {child.Position}");
                    ImGui.Text($"Timer: {child.Timer}");
                    ImGui.Text($"Render Order: {child.RenderOrder}");
                    ImGui.Text($"Angle & Rotation: {child.Angle} | {child.Rotation}");
                    if (ImGui.TreeNode($"More details for {child.InternalName}..."))
                    {
                        var properties = from p in child.GetType().GetProperties() select p;
                        foreach (var property in properties)
                        {
                            var value = property.GetValue(child, null);
                            if (value == null) value = "null";
                            ImGui.Text($"{property.Name} ({value.GetType().Name}) = {value}");
                        }
                    }
                    if (child.Children.Count != 0)
                    {
                        ImGui.Text($"Children: ({child.Children.Count})");
                        DisplayViewChildren(child.Children, _CallDepth-1);
                    }
                    ImGui.TreePop();
                }
            }
        }

        private void DisplayResourcePool(List<IResource> resPool)
        {
            ImGui.Text($"Resource Count: {resPool.Count}");
            foreach (IResource resource in resPool)
            {
                if (ImGui.TreeNode($"{resource.Name}"))
                {
                    ImGui.Text($"Indentifier: {resource.Name}");
                    ImGui.Text($"File Path: {resource.Path}");
                    ImGui.Text($"Internal Type: {resource.GetRes().GetType().Name}");

                    Type type = resource.GetRes().GetType();
                    switch (type)
                    {
                        case Type when type == typeof(Texture2D):
                            DisplayResourceTexture2D((Texture2D)resource.GetRes());
                            break;
                        case Type when type == typeof(TextureAtlas):
                            DisplayResourceTextureAtlas((TextureAtlas)resource.GetRes());
                            break;
                    }
                    ImGui.TreePop();
                }
            }
        }

        #region Resource types display

        private void DisplayResourceTexture2D(Texture2D texture)
        {
            ImGui.Text($"Texture Size: {new Vector2(texture.Width, texture.Height)}");
        }

        private void DisplayResourceTextureAtlas(TextureAtlas texture)
        {

        }

        #endregion

        private void DisplayWorld3D(World3D world)
        {
            if (ImGui.TreeNode($"World3D"))//: {world}"))
            {
                ImGui.Text($"Fog Enabled: {world.FogSettings.EnableFog}");
                ImGui.Text($"Fog Color: {world.FogSettings.FogColor}");
                ImGui.TreePop();
            }
        }
    }
}
