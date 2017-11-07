using System.Collections.Generic;
using UnityEngine;


    public class SingletonWindow<T> : Window where T : Window
    {
        protected static T ms_instance = null;
        public static T instance
        {
            get
            {
                if (ms_instance == null)
                {
                    ms_instance = new SingletonWindow<T>() as T;
                }

                return ms_instance;
            }
            set { ms_instance = value; }
        }

        protected SingletonWindow()
        {
            ms_instance = this as T;
        }
    }
