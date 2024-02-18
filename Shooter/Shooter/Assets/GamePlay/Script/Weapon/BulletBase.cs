using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;

//public class BulletBase : MonoBehaviour
//{

//    public GameObject Owner { get; private set; }
//    public Vector3 InitialPosition { get; private set; }
//    public Vector3 InitialDirection { get; private set; }
//    public Vector3 InheritedMuzzleVelocity { get; private set; }
//    public float InitialCharge { get; private set; }

//    public UnityAction OnShoot;

    
//    public void Shoot(Weapon controller)
//    {
//        Owner = controller.Owner;
//        InitialPosition = transform.position;
//        InitialDirection = transform.forward;
//        InheritedMuzzleVelocity = controller.MuzzleWorldVelocity;
//        InitialCharge = controller.CurrentCharge;
//        gameObject.SetActive(true);
//        OnShoot?.Invoke();
//    }
//}
