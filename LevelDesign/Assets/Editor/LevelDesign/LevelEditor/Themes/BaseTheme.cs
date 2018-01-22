using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace Theme
{
    public abstract class BaseTheme : ScriptableObject
    {
        private GameObject _objectToAdd;
        private bool _hasLoadedObjects;

        public virtual void ShowAddBuildings(int _numberOfRows)
        {

        }

        public virtual void ShowAddTiles(int _numberOfRows)
        {

        }

        public virtual void ShowAddProps(int _numberOfRows)
        {

        }

        public virtual GameObject ReturnObjectToAdd()
        {
            return _objectToAdd;
        }

        public virtual bool ReturnHasLoadedObjects()
        {
            return _hasLoadedObjects;
        }

        public virtual void DeleteLoadedObject()
        {
            if (_objectToAdd != null)
            {
                DestroyImmediate(_objectToAdd);
            }
        }

    }
}
