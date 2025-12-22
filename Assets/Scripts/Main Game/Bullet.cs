using System.Collections.Generic;
using Fusion;
using Unity.Mathematics;
using UnityEngine;

namespace PhotonCourse.Scripts.MainGame
{
    public class Bullet : NetworkBehaviour
    {
        [SerializeField]
        private LayerMask _playerHitBox;

        [SerializeField]
        private LayerMask _groundLayerMask;

        [SerializeField]
        private float _moveSpeed = 20.0f;

        [SerializeField]
        private float _lifeTime = 0.8f;

        [SerializeField]
        private int _bulletDamage = 10;

        [Networked]
        private TickTimer LifeTimerTimer { get; set; }
        
        [Networked]
        private NetworkBool HitSomething { get; set; }

        private Collider2D _collider;

        private List<LagCompensatedHit> _lagCompensatedHits = new List<LagCompensatedHit>();



        // Network Loop Methods--------------------------------------------------------------------

        public override void Spawned()
        {
            _collider = GetComponent<Collider2D>();

            LifeTimerTimer = TickTimer.CreateFromSeconds(Runner,_lifeTime);
        }

        public override void FixedUpdateNetwork()
        {
            if (!HitSomething)
            {
                CheckIfHitGround();
                CheckIfBulletHitAPlayer();
            }

            if (LifeTimerTimer.ExpiredOrNotRunning(Runner) == false && !HitSomething)
            {
                transform.Translate(transform.right * _moveSpeed * Runner.DeltaTime, Space.World);
            }

            if (LifeTimerTimer.Expired(Runner) || HitSomething)
            {
                LifeTimerTimer = TickTimer.None;
                Runner.Despawn(Object);
            }
        }

        // Member Methods--------------------------------------------------------------------------

        private void CheckIfHitGround()
        {
            var groundCollider = Runner.GetPhysicsScene2D().OverlapBox(transform.position, _collider.bounds.size, 0.0f, _groundLayerMask);

            if (groundCollider != default)
            {
                HitSomething = true;
            }
        }

        private void CheckIfBulletHitAPlayer()
        {
            Runner.LagCompensation.OverlapBox(transform.position, _collider.bounds.size, Quaternion.identity, Object.InputAuthority, _lagCompensatedHits, _playerHitBox);

            if (_lagCompensatedHits.Count > 0)
            {
                foreach (var hit in _lagCompensatedHits)
                {
                    if (hit.Hitbox != null)
                    {
                        var player = hit.Hitbox.GetComponentInParent<PlayerController>();
                        var bulletDidNotHitOwnPlayer = player.Object.InputAuthority.PlayerId != Object.InputAuthority.PlayerId;

                        if (bulletDidNotHitOwnPlayer && player.IsPlayerAlive)
                        {
                            if (Runner.IsServer)
                            {
                                player.GetComponent<PlayerHealthController>().Rpc_TakeDamage(_bulletDamage);
                            }

                            HitSomething = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}