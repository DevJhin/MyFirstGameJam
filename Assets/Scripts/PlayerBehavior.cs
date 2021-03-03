using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 행동 구현체
/// </summary>
public class PlayerBehavior : FieldObjectBehaviour
{

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
    private bool isGrounded;
    private bool isSideCollided;

    private Vector3 prevPos;
    private float gravityScale;
    private Vector3 currentVelocity;
    /// <summary>
    /// 최근에 플레이어 하단에 감지된 RaycastHit의 위치
    /// </summary>
    private RaycastHit2D lastGroundPoint;
    private RaycastHit2D lastSideColliderHit;

    private bool IsFalling => (prevPos.y > Transform.position.y);

    public PlayerBehavior(Player player)
    {
        this.player = player;
        Transform = player.transform;
        airDrag = 1 - player.airDrag;
        colliderExtents = player.bounds.extents;
        prevPos = Transform.position;
        ceilingReactionForceRatio = Mathf.Max(0f, player.ceilingReactionForceRatio);
    }


    public void OnPlayerUpdate()
    {

        if (player.Controller.TryGetMoveInput(out float value))
        {
            Move(value);
            player.AnimController.SetBool("IsMoving", true);
        }
        else
        {
            currentVelocity.x = 0f;
            player.AnimController.SetBool("IsMoving", false);
        }

        UpdatePhysics();

        prevPos = Transform.position;
        Transform.position += currentVelocity * Mathf.Min(0.05f, Time.deltaTime);
    }

    //Physics Manager
    private void UpdatePhysics()
    {
        // If Releasing Jump Button During Jump, Adjust Low JumpFallMultiplier.
        gravityScale = (!IsFalling && !player.Controller.IsJumpButtonOnRepeat) ? player.lowJumpFallMultiplier : player.fallMultiplier;

        bool oldIsGrounded = isGrounded;
        isGrounded = IsGrounded();

        if (isGrounded)
        {
            // 이전 프레임에서 높이가 변경되었는데 레이캐스트에 땅이 감지된 경우, 플레이어를 지형에 착지시킵니다.
            if (oldIsGrounded == false)
            {
                Transform.position = new Vector2(Transform.position.x, lastGroundPoint.point.y + colliderExtents.y);
            }
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
            currentVelocity.y += Physics2D.gravity.y * gravityScale * Time.deltaTime;

            // 상향이동 도중 천장에 부딪혔다면 반대 방향으로 튕겨나옵니다.
            if (IsHitCeiling() && currentVelocity.y > 0f)
            {
                currentVelocity.y = -currentVelocity.y * ceilingReactionForceRatio;
            }
            // 점프 최대 속도보다 빠르게 낙하하지 않습니다.
            if (currentVelocity.y < -player.jumpPower)
            {
                currentVelocity.y = -player.jumpPower;
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

        player.AnimController.SetBool("IsGrounded", isGrounded);
    }

    //Movement
    public void Move(float value)
    {
        //1. 이동 방향값에 따라 방향 전환 처리.
        if ((value < 0 && !facingLeft) || (value > 0 && facingLeft))
        {
            Flip();
        }

        //2. 측면 충돌 처리.
        float moveValue = value * player.moveSpeed * airDragMultiplier;
        isSideCollided = IsSideCollided(moveValue);
        
        //측면 충돌이 발생한 경우
        if (isSideCollided)
        {
            currentVelocity.x = 0f;

            Vector2 hitPoint = lastSideColliderHit.point;

            float xDistToHitPoint = hitPoint.x - Transform.position.x;

            //충돌 영역과 겹치는 부분의 X축 길이(겹친 길이가 음수면 서로 겹치지 않았음을 나타냄)
            float xOverlapLength =  colliderExtents.x - Mathf.Abs(xDistToHitPoint);
            
            //충돌 영역 겹침 여부를 확인.
            bool needReposition = xOverlapLength > 0;

            //겹쳤으면, 위치보정 수행.
            if (needReposition)
            {
                Vector2 newPos = Transform.position;

                //충돌 방향에 따라 겹치는 X축 길이 만큼 캐릭터 X 위치 조정.
                if (xDistToHitPoint > 0)
                {
                    newPos.x -= xOverlapLength;
                }
                else
                {
                    newPos.x += xOverlapLength;
                }

                Transform.position = newPos;
            }
        }
        else
        {
            currentVelocity.x = moveValue;
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
        //player.AttackBattleAction.Execute(player);
        player.AttackBattleActionBehaviour.Start();
    }


    /// <summary>
    /// Player의 Interact BattleAction을 실행합니다.
    /// </summary>
    public void Interact()
    {
        player.InteractActionBehaviour.Start();
    }


    //Ground Check
    private bool IsGrounded()
    {
        // Temp: 콜라이더 외곽 부분의 자연스러운 처리를 위해 캐릭터의 가로 1/4, 2/4, 3/4 지점에서만 아래쪽 지형을 체크합니다.
        Vector3 delta = -(Transform.right / 4);

        float velocityY = Mathf.Approximately(currentVelocity.y, 0f) ?
            player.raycastLength : Mathf.Max(player.raycastLength, (-currentVelocity.y + gravityScale) * Time.deltaTime);

        for (int i = 0; i < 3; i++)
        {
            Ray2D ray = new Ray2D(Transform.position + delta, Vector2.down);
            float raycastDistance = colliderExtents.y + velocityY;

            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, raycastDistance, player.groundLayerMask);

            //DrawDebugRay(ray, raycastDistance, i == 1, hit == true, GroundRayDrawOptions);

            if (hit)
            {
                lastGroundPoint = hit;
                return true;
            }
            delta += (Transform.right / 4);
            Debug.Log($"G{i} Start:{ ray.origin - (Vector2)Transform.position} End:{ray.GetPoint(raycastDistance) - (Vector2)Transform.position}Frame: {Time.frameCount}");
        }

        return false;
    }

    //Side Check
    private bool IsSideCollided(float moveValue)
    {
        // Temp: 콜라이더 외곽 부분의 자연스러운 처리를 위해 캐릭터의 세로 1/4, 2/4, 3/4 지점에서만 캐릭터 앞쪽 지형을 체크합니다.
        Vector3 delta = Vector3.down * (colliderExtents.y / 2);
        for (int i = 0; i < 3; i++)
        {
            Ray2D ray = new Ray2D(Transform.position + delta, Transform.right);
            float raycastDistance = colliderExtents.x + player.raycastLength;

            var hit = Physics2D.Raycast(ray.origin, ray.direction, raycastDistance, player.groundLayerMask);

            //DrawDebugRay(ray, raycastDistance, i == 1, hit == true, SideRayDrawOptions);

            if(hit)
            {
                lastSideColliderHit = hit;
                return true;
            }

            delta += (Vector3.up * (colliderExtents.y / 2));
            Debug.Log($"S{i} Start:{ ray.origin -(Vector2)Transform.position} End:{ray.GetPoint(raycastDistance) - (Vector2)Transform.position} Frame: {Time.frameCount}");
        }

        return false;
    }

    /// <summary>
    /// 플레이어가 천장에 부딪혔는지 감지합니다.
    /// </summary>
    private bool IsHitCeiling()
    {
        // Temp: 콜라이더 외곽 부분의 자연스러운 처리를 위해 캐릭터의 가로 1/4, 2/4, 3/4 지점에서만 위쪽 지형을 체크합니다.
        // 천장에 부딪힌 게 아니라면 점프 중에 좌우이동이 가능해야 하므로, 여기서 확인하는 높이는 IsSideCollided보다 높아야 합니다.
        Vector3 delta = -(Transform.right / 4);
        for (int i = 0; i < 3; i++)
        {
            Ray2D ray = new Ray2D(Transform.position + delta, Vector2.up);
            float raycastDistance = colliderExtents.y * (0.75f + currentVelocity.y * Time.deltaTime);

            RaycastHit2D hit = Physics2D.Raycast(ray.origin, Vector2.up, raycastDistance, player.groundLayerMask);

            //DrawDebugRay(ray, raycastDistance, i == 1, hit == true, CeilingRayDrawOptions);

            if (hit)
            {
                return true;
            }
            delta += (Transform.right / 4);
        }

        return false;
    }

    /// <summary>
    /// 디버그 Ray 그리기 설정값.
    /// </summary>

    private class DebugRayDrawOptions
    {
        public Color DefaultColor;
        public Color CenterColor;
        public Color HitColor;

        public float RayLifeTime;

        public DebugRayDrawOptions(Color defaultColor, Color centerColor, Color hitColor, float rayLifeTime)
        {
            this.DefaultColor = defaultColor;
            this.CenterColor = centerColor;
            this.HitColor = hitColor;
            this.RayLifeTime = rayLifeTime;
        }

        public readonly static DebugRayDrawOptions DefaultOptions = new DebugRayDrawOptions(Color.white, Color.yellow, Color.green, 2f);
    }

    private static readonly DebugRayDrawOptions GroundRayDrawOptions = new DebugRayDrawOptions(Color.white, Color.yellow, Color.green, 2f);
    
    private static readonly DebugRayDrawOptions CeilingRayDrawOptions = new DebugRayDrawOptions(Color.magenta, Color.red, Color.green, 2f);

    private static readonly DebugRayDrawOptions SideRayDrawOptions = new DebugRayDrawOptions(Color.cyan, Color.blue, Color.green, 2f);


    /// <summary>
    /// 디버그 Ray를 그립니다.
    /// </summary>
    /// <param name="isCenter">가운데 위치한 Ray 여부(색을 다르게 칠함).</param>
    /// <param name="isHit">Raycast의 Hit 여부(색을 다르게 칠함).</param>
    /// <param name="options">그리기 설정값.Null일 경우 디폴트 설정 사용.</param>
    private void DrawDebugRay(Ray2D ray, float raycastDistance, bool isCenter, bool isHit, DebugRayDrawOptions options = null)
    {
        if (options == null) options = DebugRayDrawOptions.DefaultOptions;

        Color rayColor = isCenter ? options.CenterColor : options.DefaultColor;
        rayColor = isHit ? options.HitColor : rayColor;

        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, rayColor, options.RayLifeTime);
    }

}
