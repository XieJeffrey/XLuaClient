using UnityEngine;
using System.Collections;


    public class SingletonManager<T> : Manager where T : Manager
    {

        protected static T ms_instance = null;
        public static T instance
        {
            get
            {
                if (ms_instance == null)
                {
                    ms_instance = new SingletonManager<T>() as T;
                }
                return ms_instance;
            }
            set { ms_instance = value; }
        }

        protected SingletonManager()
        {
            ms_instance = this as T;
        }
    }

