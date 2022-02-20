using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkOurPlayer : MonoBehaviour
{

    #region//インスペクターで調整可能な変数
    [Header("移動速度")] public float runSpeed;
    [Header("ジャンプ速度")] public float jumpSpeed;
    [Header("ジャンプ高さ制限")] public float jumpHeight;
    [Header("ジャンプ制限時間")] public float jumpLimitTime;
    [Header("重力")] public float gravity;
    [Header("地面の接地判定")]public GroundCheck ground;
    [Header("頭ぶつけた判定")] public GroundCheck head;
    public AnimationCurve dashCurve;
    public AnimationCurve jumpCurve;
    #endregion

    #region//プライベート変数
    private Animator anim = null;
    private Rigidbody rb = null;
    private bool isGround = false;
    private bool isHead = false;
    private bool isJump = false;
    private bool isRun = false;
    private float jumpPos = 0.0f;
    private float dashTime, jumpTime;
    private float beforeKey;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;   //フレームレートを60fpsに

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //接地判定を得る
        isGround = ground.IsGround();
        isHead = head.IsGround();

        //各種座標軸の速度を求める
        float xSpeed = GetXSpeed();
        float ySpeed = GetYSpeed();

        //アニメーションを適用
        SetAnimation();

        rb.velocity = new Vector3(xSpeed, ySpeed, rb.velocity.z);
    }

    /// <summary>
    /// Y成分で必要な計算をし、速度を返す
    /// </summary>
    /// <returns>Y軸の速さ</returns>
    private float GetYSpeed()
    {
        float verticalKey = Input.GetAxis("Vertical");
        float ySpeed = -gravity;

        if (isGround)
        {
            if (verticalKey > 0)
            {
                ySpeed = jumpSpeed;
                jumpPos = transform.position.y;　//ジャンプした位置を記録する
                isJump = true;
                jumpTime = 0.0f;
                Debug.Log("ジャンプ開始");
            }
            else
            {
                isJump = false;
            }
        }
        else if (isJump)
        {
            //上方向キーを押しているか
            bool pushUpKey = verticalKey > 0;
            //現在の高さが飛べる高さより下か
            bool canHeight = jumpPos + jumpHeight > transform.position.y;
            //ジャンプ時間が長くなりすぎてないか
            bool canTime = jumpLimitTime > jumpTime;

            if (pushUpKey && canHeight && canTime && !isHead)
            {
                ySpeed = jumpSpeed;
                jumpTime += Time.deltaTime;
                Debug.Log("ジャンプ中");
            }
            else
            {
                isJump = false;
                jumpTime = 0.0f;
                Debug.Log("ジャンプ終了");
            }
        }

        if (isJump)
        {
            ySpeed *= jumpCurve.Evaluate(jumpTime);
        }
        return ySpeed;
    }

    /// <summary>
    /// X成分で必要な計算を行い、速度を返す
    /// </summary>
    /// <returns>X軸の速さ</returns>
    private float GetXSpeed()
    {
        float horizontalKey = Input.GetAxis("Horizontal");
        float xSpeed = 0.0f;
        if (horizontalKey > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            isRun = true;
            dashTime += Time.deltaTime;
            xSpeed = runSpeed;
        }
        else if (horizontalKey < 0)
        {
            transform.localScale = new Vector3(1, 1, -1);
            isRun = true;
            dashTime += Time.deltaTime;
            xSpeed = -runSpeed;
        }
        else
        {
            isRun = false;
            xSpeed = 0.0f;
            dashTime = 0.0f;
        }

        //前回の入力からダッシュの反転を判断して速度を変える
        if (horizontalKey > 0 && beforeKey < 0)
        {
            dashTime = 0.0f;
        }
        else if (horizontalKey < 0 && beforeKey > 0)
        {
            dashTime = 0.0f;
        }

        beforeKey = horizontalKey;
        xSpeed *= dashCurve.Evaluate(dashTime);
        return xSpeed;
    }

    /// <summary>
    /// アニメーションを設定する
    /// </summary>
    private void SetAnimation()
    {
        anim.SetBool("IsJump", isJump);
        anim.SetBool("IsRun", isRun);
    }
}
