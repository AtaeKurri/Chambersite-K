using Chambersite_K.GameObjects;
using Chambersite_K.Graphics;
using Chambersite_K.Views;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.ImGUI
{
    internal class GUIViewAndObjectList
    {
        public void Draw()
        {
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(500, 500), ImGuiCond.Appearing);
            ImGui.Begin("Game Inspector");
            if (ImGui.BeginTabBar("##objectInspector"))
            {
                if (ImGui.BeginTabItem("Views"))
                {
                    foreach (IView view in GAME.ActiveViews)
                    {
                        if (ImGui.CollapsingHeader($"{view.InternalName} | Id:{view.Id}"))
                        {
                            DisplayView(view);
                        }
                    }
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("GameObjects"))
                {
                    List<Tuple<string, GameObjectPool>> goHolders = new List<Tuple<string, GameObjectPool>>
                    { new Tuple<string, GameObjectPool>("Global GameObjects", GAME.GlobalObjectPool) };
                    foreach (IView view in GAME.ActiveViews)
                        goHolders.Add(new Tuple<string, GameObjectPool>($"{view.InternalName} | Id:{view.Id}", view.LocalObjectPool));
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
                    List<Tuple<string, List<Resource>>> resourceHolders = new List<Tuple<string, List<Resource>>>
                    { new Tuple<string, List<Resource>>("Global Pool", GAME.GlobalResource) };
                    foreach (IView view in GAME.ActiveViews)
                        resourceHolders.Add(new Tuple<string, List<Resource>>($"{view.InternalName} | Id:{view.Id}", view.LocalResources));

                    foreach (Tuple<string, List<Resource>> container in resourceHolders)
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

            ImGui.End();
        }

        private void DisplayView(IView view)
        {
            ImGui.Text($"Type: {view.GetType().Name}");
            ImGui.Text($"View Type: {view.vType}");
            ImGui.Text($"Status: {view.ViewStatus}");
            ImGui.Text($"Timer: {view.Timer}");
            if (ImGui.TreeNode($"More details for {view.InternalName}..."))
            {
                var properties = from p in view.GetType().GetProperties() select p;
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
                DisplayChildren(goPool.ObjectPool, 20);
            }
        }

        /// <summary>
        /// Will recursively display all of a GameObject Children.<br/>
        /// To avoid possible infinite calls, this functions has a max call depth of 20.
        /// </summary>
        /// <param name="obj"></param>
        private void DisplayChildren(List<GameObject> Children, int _CallDepth)
        {
            if (_CallDepth <= 0)
                return;
            foreach (GameObject child in Children)
            {
                if (ImGui.TreeNode($"{child} (Id:{child.Id})"))
                {
                    ImGui.Text($"Status: {child.Status}");
                    ImGui.Text($"Position: {child.Position}");
                    ImGui.Text($"Timer: {child.Timer}");
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
                        DisplayChildren(child.Children, _CallDepth-1);
                    }
                    ImGui.TreePop();
                }
            }
        }

        private void DisplayResourcePool(List<Resource> resPool)
        {
            foreach (Resource resource in resPool)
            {
                ImGui.Text($"Resource Count: {resPool.Count}");
                if (ImGui.TreeNode($"{resource.Name}"))
                {
                    ImGui.Text($"Indentifier: {resource.Name}");
                    ImGui.Text($"File Path: {resource.Path}");
                    ImGui.Text($"Internal Type: {resource.Res.GetType().Name}");
                    ImGui.TreePop();
                }
            }
        }
    }
}
