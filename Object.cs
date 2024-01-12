using SFML.Graphics;

using SilverRaven.SFML.Tools;

namespace SilverRaven.SFML
{
    public class Object
    {
#region static

        private static List<Object> objects; // all current objects
        private static List<Object> newObjects; // temporary list of objects to add
        private static List<Object> destroyedObjects; // temporary list of objects to remove

        private static void RegisterObject(Object obj)
        {
            newObjects ??= new();
            newObjects.Add(obj);
        }

        private static void DestroyObject(Object obj)
        {
            destroyedObjects ??= new();
            destroyedObjects.Add(obj);
        }

        private static void HandleObjectModification()
        {
            objects ??= new();
            newObjects ??= new();
            destroyedObjects ??= new();

            objects.AddRange(newObjects);
            newObjects.Clear();

            for (int i = 0; i < destroyedObjects.Count; i++)
                objects.Remove(destroyedObjects[i]);
            destroyedObjects.Clear();
        }

        protected static List<Object> GetAllObjects() => new (objects);

#region global calls

        /// <summary>
        /// Returns the exact item passed into this method. 
        /// Create new object instances like `Object.CreateInstance(new Player());`
        /// This method soly exists to get rid of IDE warnings when creating an instance without storing it in a variable.
        /// Using `Player p = new Player();` still works.
        /// </summary>
        public static Object CreateInstance<T>(T instance) where T : Object => instance;

        /// <summary>
        /// Calls the HandleInput(input) method on each object.
        /// </summary>
        public static void HandleInputAll(InputSystem input)
        {
            objects ??= new();

            foreach (Object obj in objects)
                obj.HandleInput(input);
            
            HandleObjectModification();
        }

        /// <summary>
        /// ForceUpdates all objects. Calls the ForceUpdate() method on each object.
        /// </summary>
        public static void ForceUpdateAll()
        {
            objects ??= new();
            
            foreach (Object obj in objects)
                obj.ForceUpdate();
            
            HandleObjectModification();
        }

        /// <summary>
        /// Updates all objects. Calls the Update() method on each object.
        /// </summary>
        public static void UpdateAll()
        {
            objects ??= new();
            
            foreach (Object obj in objects)
                obj.Update();
            
            HandleObjectModification();
        }
        
        /// <summary>
        /// Draw all objects to the window. Calls the Draw(window) method on each object.
        /// </summary>
        public static void DrawAll(RenderWindow window)
        {
            objects ??= new();
            
            foreach (Object obj in objects)
                obj.Draw(window);
            
            HandleObjectModification();
        }

#endregion

#endregion

        public Object() // Constructor
        {
            RegisterObject(this);
        }

        ~Object() // Finalizer
        {
            Destroy();
        }

#region virtual methods

        /// <summary>
        /// Destroys this object
        /// </summary>
        public virtual void Destroy()
        {
            OnDestroy();
            DestroyObject(this);
        }

        /// <summary>
        /// Called before destroying this object. Cleanup code goes here.
        /// </summary>
        public virtual void OnDestroy() { }

        /// <summary>
        /// HandleInput is called before Update() and should process input, to later be used in Update.
        /// </summary>
        /// <param name="input">Input object which tracks inputs. Use GetKey(key), GetMouseButton(button) or mousePosition.</param>
        protected virtual void HandleInput(InputSystem input) { }

        /// <summary>
        /// Update gets called every frame even if the program is paused.
        /// </summary>
        protected virtual void ForceUpdate() { }

        /// <summary>
        /// Update gets called every frame unless the program is paused.
        /// </summary>
        protected virtual void Update() { }
        
        /// <summary>
        /// Used to draw the object to the window.
        /// </summary>
        protected virtual void Draw(RenderWindow window) { }

#endregion

    }
}