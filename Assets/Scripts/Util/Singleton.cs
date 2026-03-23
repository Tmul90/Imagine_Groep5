using UnityEngine;

namespace Util
{
	public abstract class Singleton<T> : MonoBehaviour where T:Component{
		private static T instance;
		public  static T  Instance 
		{
			get 
			{
				if (instance == null)
					EnsureInstantiated();

				return instance;
			}
		}
		public static bool instantiated {
			get { return instance != null; }
		}
		public static void EnsureInstantiated()
		{
			if (instance != null) return;
			// singletons can get disconnected when unity compile events occur, this will serve as a re-connection if that occurs
			instance = GameObject.FindObjectOfType<T>() as T;
			// if it's still null, see if we can instantiate it from the resources folder!
			if (instance == null) {
				GameObject obj = Resources.Load("Singletons/" + typeof(T).Name) as GameObject;
				if (obj != null) {
					GameObject go = GameObject.Instantiate(obj);
					instance = go.GetComponent<T>();
				}
			}
			// and if there was no prefab, lets just try a plain create and go!
			if (instance == null) {
				GameObject go = new GameObject("_" + typeof(T).Name);
				instance = go.AddComponent(typeof(T)) as T;
			}
			// and if all else fails, start spouting smoke
			if (instance == null) {
				Debug.LogErrorFormat("Couldn't connect or create singleton {0}!", typeof(T).Name);
			}
		}

		protected virtual void Awake() {
			if (instance != null && instance != this) {
				Debug.LogErrorFormat(gameObject, "Creating a new instance of a singleton [{0}] when one already exists!", typeof(T).Name);
				gameObject.SetActive(false);
				return;
			}
			instance = GetComponent<T>();
		}
		protected virtual void OnDestroy() {
			instance = null;
		}
	}
}
