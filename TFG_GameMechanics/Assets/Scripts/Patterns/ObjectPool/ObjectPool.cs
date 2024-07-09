using System;
using System.Collections.Generic;
using Patterns.ObjectPool.Interfaces;

namespace Patterns.ObjectPool
{
    [Serializable]
    public class ObjectPool : IObjectPool
    {
        private IPooleableObject _objectPrototype;
        private readonly bool _allowAddNew;
        
        public List<IPooleableObject> objects;
        
        private int _activeObjects;
        
        public ObjectPool(IPooleableObject objectPrototype, int initialNumberOfElements, bool allowAddNew)
        {
            _objectPrototype = objectPrototype;     // El tipo de objeto que se va a poolear
            _allowAddNew = allowAddNew;             // Si se permite agregar nuevos objetos al pool
            objects = new List<IPooleableObject>(initialNumberOfElements); // Lista de objetos pooleados
            _activeObjects = 0;                    // Número de objetos activos
            
            for (int i = 0; i < initialNumberOfElements; i++)
            {
                objects.Add(CreateObject());
            }
        }

        private IPooleableObject CreateObject()
        {
            IPooleableObject newObj = _objectPrototype.Clone() as IPooleableObject;
            return newObj;
        }

        public IPooleableObject Get()
        {
            for (int i = 0; i< objects.Count; i++)
            {
                if (!objects[i].Active)
                {
                    objects[i].Active = true;
                    _activeObjects += 1;
                    return objects[i];
                }
            }

            if (_allowAddNew)
            {
                IPooleableObject newObj = CreateObject();
                newObj.Active = true;
                objects.Add(newObj);
                _activeObjects += 1;
                return newObj;
            }
            else
            {
                IPooleableObject obj = objects[0];
                Release(obj); 
                obj.Active = true;
                _activeObjects += 1;
                objects.Remove(obj);
                objects.Add(obj);
                return obj;
            }

            return null;
        }

        public void Release(IPooleableObject obj)
        {
            obj.Active = false;
            _activeObjects -= 1;
            obj.Reset();
        }
        
        public int GetCount()
        {
            return objects.Count;
        }
        
        public int GetActive()
        {
            return _activeObjects;
        }
    }
}