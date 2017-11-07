using UnityEngine;
using System.Collections;
using System.Reflection;



    public class Utility : Singleton<Utility>
    {

        public GameObject Find(GameObject go, string childName)
        {
            foreach (Transform t in go.transform)
            {
                if (go.name.CompareTo(childName) == 0)
                {
                    return t.gameObject;
                }
            }
            Debug.LogErrorFormat("{0} 节点下找不到GameObject {1}", go.name, childName);
            return null;
        }

        public GameObject Find(string parentName, string childName)
        {
            GameObject go = GameObject.Find(parentName);
            if (go != null)
            {
                return Find(go, childName);
            }
            else
            {
                Debug.LogErrorFormat("找不到父节点：{0}", parentName);
                return null;
            }
        }

        public GameObject AddChild(string parentName, string childName, string AttachParentName)
        {
            GameObject go = Find(parentName, childName);
            if (go != null)
            {
                GameObject newParent = GameObject.Find(AttachParentName);
                if (newParent != null)
                {
                    GameObject child = Instantiate(go) as GameObject;
                    child.transform.parent = newParent.transform;
                    return child;

                }
                else
                {
                    Debug.LogErrorFormat("the attachName：{0} is not found" + AttachParentName);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public string GetColorString(string color, string content)
        {
            return string.Format("[{0}]{1}[-]", color, content);
        }

        public GameObject Instantiate(GameObject src, string name = "")
        {
            if (null == src)
                return null;
            GameObject clone = GameObject.Instantiate(src);
            if (!string.IsNullOrEmpty(name))
                clone.name = name;
            return clone;
        }
    }

