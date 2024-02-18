using SFML.Graphics;
using SFML.System;

namespace SilverRaven.SFML
{
    public class GameObject
    {

#region static

        /// <summary>List of all current objects, sorted by sortOrder (big -> small). Gets looped over in reverse (eg. in Update())</summary>
        private static List<GameObject> objects;
        /// <summary>Temporary list of all objects still to be added to the main list. Get added in HandleObjectModification()</summary>
        private static List<GameObject> newObjects;
        /// <summary>Temporary list of all objects still to be removed from the main list. Get removed in HandleObjectModification()</summary>
        private static List<GameObject> destroyedObjects;

        private static void RegisterObject(GameObject obj)
        {
            newObjects ??= new();
            newObjects.Add(obj);
        }

        private static void DestroyObject(GameObject obj)
        {
            destroyedObjects ??= new();
            destroyedObjects.Add(obj);
        }

        private static void DestroyObjectImmediately(GameObject obj)
        {
            objects.Remove(obj);
        }

        private static void HandleObjectModification()
        {
            objects ??= new();
            newObjects ??= new();
            destroyedObjects ??= new();

            foreach (GameObject obj in newObjects)
            {
                float layer = obj.sortOrder;

                if (objects.Count == 0) 
                    { objects.Add(obj); continue; }

                bool wasInserted = false;
                for (int i = 0; i < objects.Count; i++)
                {
                    if (layer > objects[i].sortOrder)
                    {
                        objects.Insert(i, obj);
                        wasInserted = true;
                        break;
                    }
                }
                if (!wasInserted) objects.Add(obj);
            }
            newObjects.Clear();

            for (int i = 0; i < destroyedObjects.Count; i++)
                objects.Remove(destroyedObjects[i]);
            destroyedObjects.Clear();
        }

        protected static List<GameObject> GetAllObjects() => new (objects);
        protected static IEnumerable<T> GetAllObjects<T>() where T : GameObject
        {
            foreach (GameObject obj in objects)
                if (obj is T) 
                    yield return obj as T;
        }

#region global calls

        /// <summary>
        /// Calls the Destroy() method on each object.
        /// </summary>
        public static void DestroyAll()
        {
            for (int i = objects.Count - 1; i >= 0 ; i--)
                objects[i].Destroy();
            
            HandleObjectModification();
        }

        /// <summary>
        /// Calls the HandleInput(input) method on each object.
        /// </summary>
        public static void HandleInputAll()
        {
            objects ??= new();

            for (int i = objects.Count - 1; i >= 0 ; i--)
                objects[i].HandleInput();
            
            HandleObjectModification();
        }

        /// <summary>
        /// ForceUpdates all objects. Calls the ForceUpdate() method on each object. Gets called after UpdateAll()
        /// </summary>
        public static void ForceUpdateAll()
        {
            objects ??= new();
            
            for (int i = objects.Count - 1; i >= 0 ; i--)
                objects[i].ForceUpdate();
            
            HandleObjectModification();
        }

        /// <summary>
        /// ForceUpdates all objects. Calls the ForceUpdate() method on each object. Gets called after UpdateAll()
        /// </summary>
        public static void FixedUpdateAll()
        {
            objects ??= new();
            
            for (int i = objects.Count - 1; i >= 0 ; i--)
                objects[i].FixedUpdate();
            
            HandleObjectModification();
        }

        /// <summary>
        /// Updates all objects. Calls the Update() method on each object. Gets called before ForceUpdateAll()
        /// </summary>
        public static void UpdateAll()
        {
            objects ??= new();

            for (int i = objects.Count - 1; i >= 0 ; i--)
                objects[i].Update();
            
            HandleObjectModification();
        }
        
        /// <summary>
        /// Draw all objects to the window. Calls the Draw(window) method on each object.
        /// </summary>
        public static void DrawAll(RenderWindow window)
        {
            objects ??= new();
            
            for (int i = objects.Count - 1; i >= 0 ; i--)
                objects[i].Draw(window);
            
            HandleObjectModification();
        }

        /// <summary>
        /// Draw all objects within the layer to the window. Calls the Draw(window) method on each object.
        /// </summary>
        /// <param name="layer">Layer to draw</param>
        public static void DrawLayer(RenderWindow window, float layer = 0f)
        {
            objects ??= new();
            
            for (int i = objects.Count - 1; i >= 0 ; i--)
                if (objects[i].sortOrder == layer)
                    objects[i].Draw(window);
            
            HandleObjectModification();
        }
        public static void DrawLayer(RenderWindow window, float? layerStart = 0f, float? layerEnd = null)
        {
            objects ??= new();

            if (layerEnd != null && layerStart != null && layerEnd < layerStart) // Swap
                (layerStart, layerEnd) = ((float)layerEnd, layerStart);
            
            for (int i = objects.Count - 1; i >= 0 ; i--)
                if (IsWithin(objects[i].sortOrder, layerStart, layerEnd))
                    objects[i].Draw(window);

            static bool IsWithin(float value, float? min = null, float? max = null)
            {
                bool minCondition = min == null || value >= min;
                bool maxCondition = max == null || value < max;
                return minCondition && maxCondition;
            }
            
            HandleObjectModification();
        }

#endregion

#endregion

        protected readonly float sortOrder;
        protected Vector2f position;
        protected string scene;
        public Vector2f Position {
            get => position;
            set {
                position = value;
                OnUpdatePosition();
            }
        }
        private bool isDestroyed;

        public GameObject(float? sortOrder = null) // Constructor
        {
            this.sortOrder = sortOrder ?? 0f;
            isDestroyed = false;
            // TODO: Fully implement SceneManager
            //scene = SceneManager.ACTIVE_SCENE.Name;
            RegisterObject(this);
        }

        //~GameObject() { } // Finalizer

#region virtual methods

        /// <summary>
        /// Destroys this object after current Update
        /// </summary>
        public virtual void Destroy()
        {
            if (isDestroyed) return;

            isDestroyed = true;
            OnDestroy();
            DestroyObject(this);
        }

        /// <summary>
        /// Destroys this object immediately
        /// </summary>
        public virtual void DestroyImmediately()
        {
            if (isDestroyed) return;

            isDestroyed = true;
            OnDestroy();
            DestroyObjectImmediately(this);
        }

        /// <summary>
        /// Called before destroying this object. Cleanup code goes here.
        /// </summary>
        protected virtual void OnDestroy() { }

        /// <summary>
        /// Called after setting the Position property.
        /// </summary>
        protected virtual void OnUpdatePosition() { }

        /// <summary>
        /// HandleInput is called before Update() and should process input, to later be used in Update.
        /// </summary>
        protected virtual void HandleInput() { }

        /// <summary>
        /// ForceUpdate gets called every frame even if the program is paused. Gets called after Update()
        /// </summary>
        protected virtual void ForceUpdate() { }

        /// <summary>
        /// FixedUpdate gets called a fixed amout of time unless the program is paused.
        /// </summary>
        protected virtual void FixedUpdate() { }

        /// <summary>
        /// Update gets called every frame unless the program is paused. Gets called before ForceUpdate()
        /// </summary>
        protected virtual void Update() { }
        
        /// <summary>
        /// Used to draw the object to the window.
        /// </summary>
        protected virtual void Draw(RenderWindow window) { }

#endregion

    }
}