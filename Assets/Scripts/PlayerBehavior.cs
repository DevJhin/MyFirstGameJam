using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 행동 구현체
/// </summary>
public class PlayerBehavior : FieldObjectBehaviour{

    Player player;
    /// <summary>
    /// 콜라이더 직경의 절반으로, 콜라이더 중심에서 외곽까지의 거리를 구할 때 사용됩니다.
    /// </summary>
    private Vector2 colliderExtents;
    private float airDragMultiplier;
    private float airDrag;
    private float jumpTimer;
    /// <summary>
    /// 천장에 부딪히면 부딪힌 속도의 일정 비율로 튕겨나오는 비율을 결정합니다.
    /// 0일 경우 얼마나 빠르게 부딪히든 동일하게 낙하하며, 1일 경우 부딪힐 당시의 속도와 동일합니다.
    /// </summary>
    private float ceilingReactionForceRatio;
    private bool facingLeft;
    private bool isMomentumXReset;

    private Vector3 prevPos;
    private float gravityScale;
    private Vector3 currentVelocity;
    /// <summary>
    /// 최근에 플레이어 하단에 감지된 RaycastHit의 위치
    /// </summary>
    private RaycastHit2D lastGroundPoint;
    private bool IsFalling => (prevPos.y > Transform.position.y);

    public PlayerBehavior(Player player) {
        this.player = player;
        Transform = player.transform;
        airDrag = 1 - player.airDrag;
        colliderExtents = player.col.bounds.extents;
        prevPos = Transform.position;
        ceilingReactionForceRatio = Mathf.Max(0f, player.ceilingReactionForceRatio);
    }

    public void OnPlayerUpdate()
    {
        if (player.Controller.TryGetMoveInput(out float value))
        {
            player.SetAnimation("run");
            Move(value);
        }
        else
		{
            currentVelocity.x = 0f;
        }
        UpdatePhysics();

        prevPos = Transform.position;
        Transform.position += currentVelocity * Time.deltaTime;
    }

    //Physics Manager
    private void UpdatePhysics()
    {
        // If Releasing Jump Button During Jump, Adjust Low JumpFallMultiplier.
        gravityScale = (!IsFalling && !player.Controller.IsJumpButtonOnRepeat) ? player.lowJumpFallMultiplier : player.fallMultiplier;
        bool isGrounded = IsGrounded();

        // 이전 프레임에서 높이가 변경되었는데 레이캐스트에 땅이 감지된 경우의 처리
        // 이번 프레임에 착지해야 한다면 착지시키고, 아니라면 IsGrounded()의 감지를 무시합니다.
        if (isGrounded && !Mathf.Approximately(prevPos.y, Transform.position.y))
        {
            // 낙하 중이고 내 위치보다 낮은 땅이 감지되면 착지합니다.
            if(IsFalling && lastGroundPoint.point.y < Transform.position.y - (colliderExtents.y * 0.5f))
			{
                Transform.position = new Vector2(Transform.position.x, lastGroundPoint.point.y + colliderExtents.y);
            }
			else
			{
                isGrounded = false;
            }
        }
        if(isGrounded)
		{
            //Reset Multiplier
            airDragMultiplier = 1;
            currentVelocity.y = 0f;
            //Jump Check
            if (jumpTimer > Time.time)
            {
                Jump();
            }
        }
        else
        {
            // 점프 최대 속도보다 빠르게 낙하하지 않습니다.
            if(currentVelocity.y > -player.jumpPower)
			{
                currentVelocity.y += Physics2D.gravity.y * gravityScale * Time.deltaTime;

                // 상향이동 도중 천장에 부딪혔다면 반대 방향으로 튕겨나옵니다.
                if (IsHitCeiling() && currentVelocity.y > 0f)
                {
                    currentVelocity.y = -currentVelocity.y * ceilingReactionForceRatio;
                }
            }

            //Air Drag Check
            if (!player.Controller.IsMoveButtonOnRepeat && !isMomentumXReset)
            {
                isMomentumXReset = true;
            }
            //Air Drag
            if (isMomentumXReset)
            {
                airDragMultiplier = airDrag;
            }
        }
    }

    //Movement
    public void Move(float value)
    {
        currentVelocity.x = IsSideCollided() ?
            0f : (value * player.moveSpeed * airDragMultiplier);

        if ((value < 0 && !facingLeft) || (value > 0 && facingLeft))
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingLeft = !facingLeft;
        Transform.rotation = Quaternion.Euler(0, facingLeft ? 180 : 0, 0);
    }

    //Jump
    public void CheckJump()
    {
        jumpTimer = Time.time + player.jumpCheckTimer;
    }

    // TODO: 점프 중 천장에 부딪히게 하는 처리
    private void Jump()
    {
        currentVelocity.y = player.jumpPower;
        jumpTimer = 0;
        isMomentumXReset = false;

        player.SetAnimation("jump");
    }

    /// <summary>
    /// Player의 Attack BattleAction을 실행합니다.
    /// </summary>
    public void Attack()
    {
        player.AttackBattleAction.Execute(player);
    }

    //Ground Check
    private bool IsGrounded()
    {
        Debug.DrawRay(Transform.position, Vector2.down * (colliderExtents.y + player.raycastLength), Color.yellow, 3f);
        Debug.DrawRay(Transform.position + (Transform.right / 3), Vector2.down * (colliderExtents.y + player.raycastLength), Color.red, 3f);
        Debug.DrawRay(Transform.position - (Transform.right / 3), Vector2.down * (colliderExtents.y + player.raycastLength), Color.red, 3f);

        // Temp: 콜라이더 외곽 부분의 자연스러운 처리를 위해 캐릭터의 가로 1/6, 3/6, 5/6 지점에서만 아래쪽 지형을 체크합니다.
        Vector3 delta = -(Transform.right / 3);
        for (int i = 0; i < 3; i++)
		{
            RaycastHit2D hit = Physics2D.Raycast(Transform.position + delta, Vector2.down, colliderExtents.y + (Time.deltaTime * gravityScale), player.groundLayerMask);
            if (hit)
			{
                lastGroundPoint = hit;
                return true;
			}
            delta += (Transform.right / 3);
        }

        return false;
    }

    //Side Check
    private bool IsSideCollided()
    {
        Debug.DrawRay(Transform.position + (Vector3.up * (colliderExtents.y / 2)), Transform.right * (colliderExtents.x + player.raycastLength), Color.green, 3f);
        Debug.DrawRay(Transform.position, Transform.right * (colliderExtents.x + player.raycastLength), Color.cyan, 3f);
        Debug.DrawRay(Transform.position + (Vector3.down * (colliderExtents.y /2)), Transform.right * (colliderExtents.x + player.raycastLength), Color.green, 3f);

        // Temp: 콜라이더 외곽 부분의 자연스러운 처리를 위해 캐릭터의 세로 1/4, 2/4, 3/4 지점에서만 캐릭터 앞쪽 지형을 체크합니다.
        Vector3 delta = Vector3.down * (colliderExtents.y / 2);
        for (int i = 0; i < 3; i++)
        {
            if (Physics2D.Raycast(Transform.position + delta, Transform.right, colliderExtents.y + player.raycastLength, player.groundLayerMask))
            {
                return true;
            }
            delta += (Vector3.up * (colliderExtents.y / 2));
        }

        return false;
    }

    /// <summary>
    /// 플레이어가 천장에 부딪혔는지 감지합니다.
    /// </summary>
    private bool IsHitCeiling()
	{
        Debug.DrawRay(Transform.position, Vector2.up * colliderExtents.y * (0.75f + currentVelocity.y * Time.deltaTime), Color.magenta, 3f);
        Debug.DrawRay(Transform.position + (Transform.right / 3), Vector2.up * colliderExtents.y * (0.75f + currentVelocity.y * Time.deltaTime), Color.red, 3f);
        Debug.DrawRay(Transform.position - (Transform.right / 3), Vector2.up * colliderExtents.y * (0.75f + currentVelocity.y * Time.deltaTime), Color.red, 3f);

        // Temp: 콜라이더 외곽 부분의 자연스러운 처리를 위해 캐릭터의 가로 1/6, 3/6, 5/6 지점에서만 위쪽 지형을 체크합니다.
        // 천장에 부딪힌 게 아니라면 점프 중에 좌우이동이 가능해야 하므로, 여기서 확인하는 높이는 IsSideCollided보다 높아야 합니다.
        Vector3 delta = -(Transform.right / 3);
        for (int i = 0; i < 3; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(Transform.position + delta, Vector2.up, colliderExtents.y * (0.75f + currentVelocity.y * Time.deltaTime), player.groundLayerMask);
            if (hit)
            {
                return true;
            }
            delta += (Transform.right / 3);
        }

        return false;
    }
}
