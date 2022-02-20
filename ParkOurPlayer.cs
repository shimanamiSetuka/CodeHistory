using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkOurPlayer : MonoBehaviour
{

    #region//�C���X�y�N�^�[�Œ����\�ȕϐ�
    [Header("�ړ����x")] public float runSpeed;
    [Header("�W�����v���x")] public float jumpSpeed;
    [Header("�W�����v��������")] public float jumpHeight;
    [Header("�W�����v��������")] public float jumpLimitTime;
    [Header("�d��")] public float gravity;
    [Header("�n�ʂ̐ڒn����")]public GroundCheck ground;
    [Header("���Ԃ�������")] public GroundCheck head;
    public AnimationCurve dashCurve;
    public AnimationCurve jumpCurve;
    #endregion

    #region//�v���C�x�[�g�ϐ�
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
        Application.targetFrameRate = 60;   //�t���[�����[�g��60fps��

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //�ڒn����𓾂�
        isGround = ground.IsGround();
        isHead = head.IsGround();

        //�e����W���̑��x�����߂�
        float xSpeed = GetXSpeed();
        float ySpeed = GetYSpeed();

        //�A�j���[�V������K�p
        SetAnimation();

        rb.velocity = new Vector3(xSpeed, ySpeed, rb.velocity.z);
    }

    /// <summary>
    /// Y�����ŕK�v�Ȍv�Z�����A���x��Ԃ�
    /// </summary>
    /// <returns>Y���̑���</returns>
    private float GetYSpeed()
    {
        float verticalKey = Input.GetAxis("Vertical");
        float ySpeed = -gravity;

        if (isGround)
        {
            if (verticalKey > 0)
            {
                ySpeed = jumpSpeed;
                jumpPos = transform.position.y;�@//�W�����v�����ʒu���L�^����
                isJump = true;
                jumpTime = 0.0f;
                Debug.Log("�W�����v�J�n");
            }
            else
            {
                isJump = false;
            }
        }
        else if (isJump)
        {
            //������L�[�������Ă��邩
            bool pushUpKey = verticalKey > 0;
            //���݂̍�������ׂ鍂����艺��
            bool canHeight = jumpPos + jumpHeight > transform.position.y;
            //�W�����v���Ԃ������Ȃ肷���ĂȂ���
            bool canTime = jumpLimitTime > jumpTime;

            if (pushUpKey && canHeight && canTime && !isHead)
            {
                ySpeed = jumpSpeed;
                jumpTime += Time.deltaTime;
                Debug.Log("�W�����v��");
            }
            else
            {
                isJump = false;
                jumpTime = 0.0f;
                Debug.Log("�W�����v�I��");
            }
        }

        if (isJump)
        {
            ySpeed *= jumpCurve.Evaluate(jumpTime);
        }
        return ySpeed;
    }

    /// <summary>
    /// X�����ŕK�v�Ȍv�Z���s���A���x��Ԃ�
    /// </summary>
    /// <returns>X���̑���</returns>
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

        //�O��̓��͂���_�b�V���̔��]�𔻒f���đ��x��ς���
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
    /// �A�j���[�V������ݒ肷��
    /// </summary>
    private void SetAnimation()
    {
        anim.SetBool("IsJump", isJump);
        anim.SetBool("IsRun", isRun);
    }
}
