using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    [Header("Arms")]
    [Tooltip("裝有槍支攝像頭的變換組件."), SerializeField]
    private Transform arms;

    [Tooltip("槍械攝影機相對於fps控制器GameObject的位置"), SerializeField]
    private Vector3 armPosition;

    [Header("Audio Clips")]
    [Tooltip("走路時播放的音頻片段"), SerializeField]
    private AudioClip walkingSound;

    [Tooltip("跑步時播放的音頻片段."), SerializeField]
    private AudioClip runningSound;

    [Header("Movement Settings")]
    [Tooltip("玩家在行走時移動的速度"), SerializeField]
    private float walkingSpeed = 5f;

    [Tooltip("玩家在跑步時移動的速度"), SerializeField]
    private float runningSpeed = 9f;

    [Tooltip("玩家達到最大跑步或步行速度所需的時間"), SerializeField]
    private float movementSmoothness = 0.125f;

    [Tooltip("跳躍時施加在玩家身上的力量大小"), SerializeField]
    private float jumpForce = 35f;

    [Header("Look Settings")]
    [Tooltip("fps控制器的轉速"), SerializeField]
    private float mouseSensitivity = 7f;

    [Tooltip("fps控制器達到最大轉速大約需要花費的時間"), SerializeField]
    private float rotationSmoothness = 0.05f;

    [Tooltip("臂和攝影機在x軸上的最小旋轉"),SerializeField]
    private float minVerticalAngle = -90f;

    [Tooltip("臂和攝影機在x軸上的最大旋轉"),SerializeField]
    private float maxVerticalAngle = 90f;

    [Tooltip("Unity輸入管理器的軸和按鈕的名稱。"), SerializeField]
    private FpsInput input;


    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;
    private AudioSource _audioSource;
    private SmoothRotation _rotationX;
    private SmoothRotation _rotationY;
    private SmoothVelocity _velocityX;
    private SmoothVelocity _velocityZ;
    private bool _isGrounded;

    private readonly RaycastHit[] _groundCastResults = new RaycastHit[8];
    private readonly RaycastHit[] _wallCastResults = new RaycastHit[8];


    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _collider = GetComponent<CapsuleCollider>();
        _audioSource = GetComponent<AudioSource>();
        arms = AssignCharactersCamera();
        _audioSource.clip = walkingSound;
        _audioSource.loop = true;
        _rotationX = new SmoothRotation(RotationXRaw);
        _rotationY = new SmoothRotation(RotationYRaw);
        _velocityX = new SmoothVelocity();
        _velocityZ = new SmoothVelocity();
        Cursor.lockState = CursorLockMode.Locked;//滑鼠鎖定模式
        ValidateRotationRestriction();
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

    //檢查角色是否在地面上
    //private void OnCollisionStay()
    //{
    //    var bounds = _collider.bounds;
    //    var extents = bounds.extents;
    //    var radius = extents.x - 0.01f;
    //    Physics.SphereCastNonAlloc(bounds.center, radius, Vector3.down,
    //        _groundCastResults, extents.y - radius * 0.5f, ~0, QueryTriggerInteraction.Ignore);
    //    if (!_groundCastResults.Any(hit => hit.collider != null && hit.collider != _collider)) return;
    //    for (var i = 0; i < _groundCastResults.Length; i++)
    //    {
    //        _groundCastResults[i] = new RaycastHit();
    //    }

    //    _isGrounded = true;
    //}
    //每隔固定的幀率幀處理一次角色移動和攝像機旋轉。
    private void FixedUpdate()
    {
        //使用FixedUpdate而不是Update，因為此代碼處理物理和平滑。
        RotateCameraAndCharacter();
        MoveCharacter();
        _isGrounded = false;
    }
    ///將相機移到角色上，處理跳躍並每幀播放聲音。
    void Update()
    {
        arms.position = transform.position + transform.TransformVector(armPosition);
        Jump();
        PlayFootstepSounds();
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

    private void MoveCharacter()
    {
        var direction = new Vector3(input.Move, 0f, input.Strafe).normalized;
        var worldDirection = transform.TransformDirection(direction);   //轉換世界方向
        var velocity = worldDirection * (input.Run ? runningSpeed : walkingSpeed); //速度=世界方向*(按跑步?跑步速度:走路速度)
        //檢查是否有碰撞，以使角色在撞牆時不會卡住。
        //var intersectsWall = CheckCollisionsWithWalls(velocity);
        //if (intersectsWall)
        //{
        //    _velocityX.Current = _velocityZ.Current = 0f;
        //    return;
        //}

        var smoothX = _velocityX.Update(velocity.x, movementSmoothness);
        var smoothZ = _velocityZ.Update(velocity.z, movementSmoothness);
        var rigidbodyVelocity = _rigidbody.velocity;
        var force = new Vector3(smoothX - rigidbodyVelocity.x, 0f, smoothZ - rigidbodyVelocity.z);
        _rigidbody.AddForce(force, ForceMode.VelocityChange);
    }

    //檢查與牆的碰撞
    //private bool CheckCollisionsWithWalls(Vector3 velocity) 
    //{
    //    if (_isGrounded) return false;
    //    var bounds = _collider.bounds;
    //    var radius = _collider.radius;
    //    var halfHeight = _collider.height * 0.5f - radius * 1.0f;
    //    var point1 = bounds.center;
    //    point1.y += halfHeight;
    //    var point2 = bounds.center;
    //    point2.y -= halfHeight;
    //    Physics.CapsuleCastNonAlloc(point1, point2, radius, velocity.normalized, _wallCastResults,
    //        radius * 0.04f, ~0, QueryTriggerInteraction.Ignore);
    //    var collides = _wallCastResults.Any(hit => hit.collider != null && hit.collider != _collider);
    //    if (!collides) return false;
    //    for (var i = 0; i < _wallCastResults.Length; i++)
    //    {
    //        _wallCastResults[i] = new RaycastHit();
    //    }

    //    return true;
    //}

    private void Jump()
    {
        if (!_isGrounded || !input.Jump) return;
        _isGrounded = false;
        _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void PlayFootstepSounds()
    {
        if (_isGrounded && _rigidbody.velocity.sqrMagnitude > 0.1f)
        {
            _audioSource.clip = input.Run ? runningSound : walkingSound;
            if (!_audioSource.isPlaying)
            {
                _audioSource.Play();
            }
        }
        else
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Pause();
            }
        }
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

    /// 一個幫助平滑動作的助手
    private class SmoothVelocity
    {
        private float _current;
        private float _currentVelocity;

        /// 返回平滑的速度
        public float Update(float target, float smoothTime)
        {
            return _current = Mathf.SmoothDamp(_current, target, ref _currentVelocity, smoothTime);
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

        [Tooltip("映射的虛擬軸名稱，用於來回移動字符。"),
         SerializeField]
        private string move = "Horizontal";

        [Tooltip("映射的虛擬軸名稱，用於左右移動字符。"),
         SerializeField]
        private string strafe = "Vertical";

        [Tooltip("映射為運行的虛擬按鈕的名稱."),
         SerializeField]
        private string run = "Fire3";

        [Tooltip("映射為跳轉的虛擬按鈕的名稱。"),
         SerializeField]
        private string jump = "Jump";

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

        /// 返回映射的虛擬軸的值，以前後移動字符。       
        public float Move
        {
            get { return Input.GetAxisRaw(move); }
        }

        /// 返回映射的虛擬軸的值，以左右移動字符。  
        public float Strafe
        {
            get { return Input.GetAxisRaw(strafe); }
        }

        /// 按下要運行的虛擬按鈕時，返回true。 
        public bool Run
        {
            get { return Input.GetButton(run); }
        }

        /// 在用戶按下映射為跳轉的虛擬按鈕的幀期間，返回true。    
        public bool Jump
        {
            get { return Input.GetButtonDown(jump); }
        }
    }



}
