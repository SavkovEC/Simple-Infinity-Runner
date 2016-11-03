using UnityEngine;

public class PoolableObjectInfo : MonoBehaviour {

	public ObjectPool poolReference;
	public bool inPool;
	public float distanceToPutToPoolX;
	public float distanceToPutToPoolY;
	public float distanceToPutToPoolZ = -30;
	public bool checkX;
	public bool checkY;
	public bool checkZ = true;
	public bool isDisappearDistanceZFixed;
	public bool checkLifeTime;
	public float lifeTime;

	private float lifeTimeCounter;

	Transform cachedTransform;

	Transform CachedTransform {
		get {
			if (cachedTransform == null) {
				cachedTransform = transform;
			}
			return cachedTransform;
		}
	}

	public void ReturnToPool() {
		if ((poolReference != null) && !inPool) {
			poolReference.Push(gameObject);
		}
	}

	public ObjectPool GetPool() {
		CheckPool();		
		return poolReference;
	}

	void CheckPool() {
		if (poolReference == null) {
			poolReference = PoolManager.Instance.PoolForObject(gameObject);
		}
	}

	void Update() {
		if (!inPool && (checkX || checkY || checkZ)) {
			var position = CachedTransform.position;
			if ((checkX && (position.x < distanceToPutToPoolX)) ||
			    (checkY && (position.y < distanceToPutToPoolY)) ||
			    (checkZ && (position.z < distanceToPutToPoolZ)))
			{
				gameObject.ReturnToPool();
			}
		}
		if (!inPool && checkLifeTime)
		{
			lifeTimeCounter += Time.deltaTime;
			if (lifeTimeCounter >= lifeTime)
			{
				lifeTimeCounter = 0;
				gameObject.ReturnToPool();
			}
		}
	}
}
