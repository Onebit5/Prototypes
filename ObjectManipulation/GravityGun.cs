using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityGun : MonoBehaviour
{
    public float grabDistance = 10f;
    public float throwForce = 10f;
    private GameObject grabbedObject;
    private Rigidbody grabbedObjectRigidbody;
    private Transform originalParent;
    private CollisionDetectionMode originalCollisionDetectionMode;
    private Vector3 objectOffset;

    void Update()
    {
        if (grabbedObject != null)
        {
            Vector3 desiredPosition = Camera.main.transform.position + Camera.main.transform.forward * grabDistance;
            Vector3 directionToObject = (grabbedObject.transform.position - Camera.main.transform.position).normalized;
            float distanceToObject = Vector3.Distance(Camera.main.transform.position, grabbedObject.transform.position);

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, directionToObject, out hit, distanceToObject))
            {
                if (hit.collider.gameObject != grabbedObject)
                {
                    desiredPosition = hit.point;
                }
            }

            grabbedObject.transform.position = Vector3.Lerp(grabbedObject.transform.position, desiredPosition, Time.deltaTime * 10f);
        }

        if (Input.GetMouseButtonDown(0)) // Botón izquierdo del ratón para agarrar o soltar objetos
        {
            if (grabbedObject == null)
            {
                GrabObject();
            }
            else
            {
                ReleaseObject();
            }
        }

        if (Input.GetMouseButtonDown(1) && grabbedObject != null) // Botón derecho del ratón para lanzar objetos
        {
            ThrowObject();
        }
    }

    void GrabObject()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, grabDistance))
        {
            GameObject hitObject = hit.collider.gameObject;
            Rigidbody hitObjectRigidbody = hitObject.GetComponent<Rigidbody>();

            if (hitObjectRigidbody != null)
            {
                grabbedObject = hitObject;
                grabbedObjectRigidbody = hitObjectRigidbody;
                originalParent = grabbedObject.transform.parent;

                originalCollisionDetectionMode = grabbedObjectRigidbody.collisionDetectionMode;
                grabbedObjectRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                grabbedObject.transform.parent = this.transform;
                grabbedObjectRigidbody.isKinematic = true;
            }
        }
    }

    void ReleaseObject()
    {
        if (grabbedObject != null)
        {
            grabbedObject.transform.parent = originalParent;
            grabbedObjectRigidbody.isKinematic = false;

            grabbedObjectRigidbody.collisionDetectionMode = originalCollisionDetectionMode;

            grabbedObject = null;
            grabbedObjectRigidbody = null;
            originalParent = null;
        }
    }

    void ThrowObject()
    {
        if (grabbedObject != null)
        {
            grabbedObject.transform.parent = originalParent;
            grabbedObjectRigidbody.isKinematic = false;

            Vector3 throwDirection = Camera.main.transform.forward;
            grabbedObjectRigidbody.AddForce(throwDirection * throwForce, ForceMode.Impulse);

            grabbedObjectRigidbody.collisionDetectionMode = originalCollisionDetectionMode;

            grabbedObject = null;
            grabbedObjectRigidbody = null;
            originalParent = null;
        }
    }
}
