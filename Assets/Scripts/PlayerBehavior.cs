using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 행동 구현체
/// </summary>
public class PlayerBehavior : FieldObjectBehaviour{

    Player player;
    private Vector2 colliderExtents;        // 콜라이더 직경의 절반으로, 콜라이더 중심에서 외곽까지의 거리를 구할 때 사용됩니다.
    private float airDragMultiplier;
    private float airDrag;
    private float jumpTimer;
    private bool facingLeft;
    private bool isMomentumXReset;

    private Vector3 prevPos;
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
    }

    public void OnPlayerUpdate() 
    {
        if (player.Controller.TryGetMoveInput(out float value))
        {
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
            // Releasing Jump Button During Jump
            float gravityScale = (!IsFalling && !player.Controller.IsJumpButtonOnRepeat) ? player.lowJumpFallMultiplier : player.fallMultiplier;

            // TODO: 최대 낙하속도 정해서 Player에 추가하고 사용할 예정 
            // (현재는 임시로 점프 최초 속도보다 빠르게 떨어지면, 중력 계수를 1/3로 떨어트림)
            if (currentVelocity.y < -player.jumpPower)
			{
                gravityScale /= 3f;
            }

            currentVelocity.y += Physics2D.gravity.y * gravityScale * Mathf.Min(0.05f, Time.deltaTime);
            

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
            RaycastHit2D hit = Physics2D.Raycast(Transform.position + delta, Vector2.down, colliderExtents.y + player.raycastLength, player.groundLayerMask);
            if (hit)
			{
                lastGroundPoint = hit;
                return true;
			}
            delta += (Transform.right / 3);
        }

        //Physics2D.BoxCast(player.col.bounds.center, player.col.size, 0f, Vector2.down, player.raycastLength, player.groundLayerMask);
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

        //bool hit = Physics2D.BoxCast(player.col.bounds.center, player.col.size, 0f, Transform.right, player.raycastLength, player.groundLayerMask);
        return false;
    }
}