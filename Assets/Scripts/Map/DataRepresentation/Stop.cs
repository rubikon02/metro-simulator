using Map.Data;
using UnityEngine;

namespace Map.DataRepresentation {
    public class Stop : MapElement {
        public Coordinates coordinates;
        public StopGroup group;
        public SubwayLine line;

        private float initialColliderSize = -1f;
        [SerializeField] private BoxCollider collider;
        private float factor = 1f;

        public void SetGroup(StopGroup newGroup) {
            group = newGroup;
        }

        private void Start() {
            var size = collider.size;
            initialColliderSize = size.x;

            size = new Vector3(
                initialColliderSize * factor,
                size.y,
                initialColliderSize * factor
            );
            collider.size = size;
        }

        public void ResizeCollider(float factor) {
            this.factor = factor;

            if (initialColliderSize < 0) return;

            collider.size = new Vector3(
                initialColliderSize * factor,
                collider.size.y,
                initialColliderSize * factor
            );
        }
    }
}
