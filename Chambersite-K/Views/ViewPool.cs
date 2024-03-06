using Chambersite_K.GameObjects;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Views
{
    public sealed class ViewPool(MainProcess parentProcess) : IEnumerable<View>
    {
        public MainProcess Parent { get; set; } = parentProcess;
        public List<View> Pool { get; set; } = [];
        private HashSet<Guid> UsedGuids = [];

        public void BeforeUpdate()
        {
            foreach (View view in Pool)
            {
                if (!view.WasInitialized && Parent.LoadingScreen == null)
                    view.Initialize();
                if (view.IsValid() && view.ViewStatus != ViewStatus.Paused) view.BeforeUpdate();
            }
        }

        public void Update()
        {
            foreach (View view in Pool)
            {
                if (view.IsValid() && view.ViewStatus != ViewStatus.Paused) view.Update();
            }
        }

        public void AfterUpdate()
        {
            foreach (View view in Pool)
            {
                if (view.IsValid() && view.ViewStatus != ViewStatus.Paused) view.AfterUpdate();
            }
        }

        public void Draw()
        {
            RenderViewsByType();
        }

        private void RenderViewsByType()
        {
            ViewType[] types = (ViewType[])Enum.GetValues(typeof(ViewType));
            foreach (ViewType type in types)
            {
                foreach (View view in Pool.FindAll(x => x.ViewType == type))
                    if (view.IsValid() && !view.Hidden) view.Draw();
            }
        }

        /// <summary>
        /// Create and adds a view to the active view pool.<br/>
        /// Will only call <see cref="View.Init"/> is the game is properly Initialized.
        /// </summary>
        /// <param name="view"></param>
        /// <returns>The view instance.</returns>
        /// <exception cref="InvalidViewOperationException">Will be thrown if another stage already exists or if you try to add a Loading Screen.</exception>
        public View AddView(View view)
        {
            if (view == null)
                throw new ApplicationException($"The View with a type of {view.GetType().Name} couldn't be created.");
            if (view.ViewType == ViewType.Stage && Pool.Any(x => x.ViewType == ViewType.Stage))
                throw new InvalidViewOperationException("Multiple stages cannot co-exist. If you wish to switch to another stage, please use the correct method.");
            else if (view.ViewType == ViewType.Background && Pool.Any(x => x.ViewType == ViewType.Background))
                throw new InvalidViewOperationException("Multiple Backgrounds cannot co-exist.");

            GenerateGuid(ref view);
            Pool.Add(view);
            if (view.RenderOrder == null)
                view.RenderOrder = Pool.Count - 1;
            if (Parent._IsInitialized)
                view.Initialize();

            Pool.Sort((x, y) => Comparer<int>.Default.Compare((int)x.RenderOrder, (int)y.RenderOrder));
            return view;
        }

        private void GenerateGuid(ref View v)
        {
            Guid uuid = Guid.NewGuid();
            while (UsedGuids.Contains(uuid))
                uuid = Guid.NewGuid();

            v.Id = uuid;
            UsedGuids.Add(uuid);
        }

        /// <summary>
        /// Attepts to create a new Stage and delete the existing one. Will fail if the view type provided is not of <see cref="ViewType.Stage"/>.
        /// </summary>
        /// <param name="view">An instance of <see cref="View"/> of type <see cref="ViewType.Stage"/></param>
        /// <returns>The instance of the stage.</returns>
        /// <exception cref="InvalidViewOperationException">Will be thrown if the view type is not a Stage View.</exception>
        public View SwitchToStage(View view)
        {
            if (view.ViewType != ViewType.Stage)
                throw new InvalidViewOperationException("You cannot switch to a non-stage view.");

            Pool.RemoveAll(x => x.ViewType == ViewType.Stage);
            Pool.Add(view);
            if (Parent._IsInitialized)
                view.Initialize();
            return view;
        }

        public IEnumerator<View> GetEnumerator()
        {
            return Pool.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
