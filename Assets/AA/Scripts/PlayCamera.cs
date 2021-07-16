using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCamera : MonoBehaviour
{
    [Header("Arms")]
    [Tooltip("裝有槍支攝像頭的變換組件."), SerializeField]
    private Transform arms;

    [Tooltip("槍械攝影機相對於fps控制器GameObject的位置"), SerializeField]
    private Vector3 armPosition;

    [Header("Look Settings")]
    [Tooltip("fps控制器的轉速"), SerializeField]
    private float mouseSensitivity = 7f;

    [Tooltip("fps控制器達到最大轉速大約需要花費的時間"), SerializeField]
    private float rotationSmoothness = 0.05f;

    [Tooltip("臂和攝影機在x軸上的最小旋轉"), SerializeField]
    private float minVerticalAngle = -90f;

    [Tooltip("臂和攝影機在x軸上的最大旋轉"), SerializeField]
    private float maxVerticalAngle = 90f;

    [Tooltip("Unity輸入管理器的軸和按鈕的名稱。"), SerializeField]
    private FpsInput input;

    private SmoothRotation _rotationX;
    private SmoothRotation _rotationY;

    void Start()
    {
        _rotationX = new SmoothRotation(RotationXRaw);
        _rotationY = new SmoothRotation(RotationYRaw);
        Cursor.lockState = CursorLockMode.Locked;//滑鼠鎖定模式

    }
    private Transform AssignCharactersCamera() //分配角色相機?
    {
        var t = transform;
        arms.SetPositionAndRotation(t.position, t.rotation);
        return arms;
    }
    ///將<see cref =“ minVerticalAngle” />和<see cref =“ maxVerticalAngle” />固定為有效值，並
    ///確保<see cref =“ minVerticalAngle” />小於<see cref =“ maxVerticalAngle” />。
    private void ValidateRotationRestriction() //驗證轉動限制
    {
        minVerticalAngle = ClampRotationRestriction(minVerticalAngle, -90, 90);
        maxVerticalAngle = ClampRotationRestriction(maxVerticalAngle, -90, 90);
        if (maxVerticalAngle >= minVerticalAngle) return;
        Debug.LogWarning("最大垂直角度應大於最小垂直角度");
        var min = minVerticalAngle;
        minVerticalAngle = maxVerticalAngle;
        maxVerticalAngle = min;
    }
    private static float ClampRotationRestriction(float rotationRestriction, float min, float max) //夾鉗旋轉限制
    {
        if (rotationRestriction >= min && rotationRestriction <= max) return rotationRestriction;
        var message = string.Format("Rotation restrictions should be between {0} and {1} degrees.", min, max);
        Debug.LogWarning(message);
        return Mathf.Clamp(rotationRestriction, min, max);
    }
    //每隔固定的幀率幀處理一次角色移動和攝像機旋轉。
    private void FixedUpdate()
    {
        //使用FixedUpdate而不是Update，因為此代碼處理物理和平滑。
        RotateCameraAndCharacter();
    }
    void Update()
    {
        arms.position = transform.position + transform.TransformVector(armPosition);
    }
    private void RotateCameraAndCharacter()
    {
        var rotationX = _rotationX.Update(RotationXRaw, rotationSmoothness);
        var rotationY = _rotationY.Update(RotationYRaw, rotationSmoothness);
        var clampedY = RestrictVerticalRotation(rotationY);
        _rotationY.Current = clampedY;
        var worldUp = arms.InverseTransformDirection(Vector3.up);
        var rotation = arms.rotation *
                       Quaternion.AngleAxis(rotationX, worldUp) *
                       Quaternion.AngleAxis(clampedY, Vector3.left);
        transform.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
        arms.rotation = rotation;
    }
    //不進行平滑處理，返回攝像機圍繞y軸的目標旋轉
    private float RotationXRaw
    {
        get { return input.RotateX * mouseSensitivity; }
    }
    //不進行平滑處理，返回攝像機圍繞x軸的目標旋轉。
    private float RotationYRaw
    {
        get { return input.RotateY * mouseSensitivity; }
    }
    //限制攝像機繞x軸的旋轉
    ///在<see cref =“ minVerticalAngle” />和<see cref =“ maxVerticalAngle” />值之間。
    private float RestrictVerticalRotation(float mouseY)
    {
        var currentAngle = NormalizeAngle(arms.eulerAngles.x);
        var minY = minVerticalAngle + currentAngle;
        var maxY = maxVerticalAngle + currentAngle;
        return Mathf.Clamp(mouseY, minY + 0.01f, maxY - 0.01f);
    }
    ///標準化-180至180度之間的角度。
    /// <param name =“ angleDegrees”>要歸一化的角度</ param>
    /// <returns>歸一化的角度</ returns>
    private static float NormalizeAngle(float angleDegrees)
    {
        while (angleDegrees > 180f)
        {
            angleDegrees -= 360f;
        }

        while (angleDegrees <= -180f)
        {
            angleDegrees += 360f;
        }

        return angleDegrees;
    }
    //一個有助於平滑相機旋轉的助手
    private class SmoothRotation
    {
        private float _current;
        private float _currentVelocity;

        public SmoothRotation(float startAngle)
        {
            _current = startAngle;
        }

        ///返回平滑的旋轉。
        public float Update(float target, float smoothTime)
        {
            return _current = Mathf.SmoothDampAngle(_current, target, ref _currentVelocity, smoothTime);
        }

        public float Current
        {
            set { _current = value; }
        }
    }
    /// 輸入映射
    //[Serializable]
    private class FpsInput
    {
        [Tooltip("映射為圍繞y軸旋轉相機的虛擬軸的名稱"),
        SerializeField]
        private string rotateX = "Mouse X";

        [Tooltip("映射為圍繞x軸旋轉相機的虛擬軸的名稱"),
         SerializeField]
        private string rotateY = "Mouse Y";

        /// 返回虛擬軸的值，該虛擬軸被映射為圍繞y軸旋轉相機。
        public float RotateX
        {
            get { return Input.GetAxisRaw(rotateX); }
        }

        ///返回虛擬軸的值，該虛擬軸被映射為圍繞x軸旋轉相機。       
        public float RotateY
        {
            get { return Input.GetAxisRaw(rotateY); }
        }

    }
}
