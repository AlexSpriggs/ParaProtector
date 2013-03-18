using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ParaProtector
{
    public class CollisionDelegate
    {
        public CollisionDelegate()
        {
            data = null;
            shape = null;
        }

        public CollisionDelegate(object data, Shape shape)
        {
            this.data = data;
            this.shape = shape;
        }

        public object data;
        public Shape shape;
        public bool enabled = true;
    }

    /// <summary>
    /// Object used to reference, access, and manipulate a collision delegate that has been registered within a collision world. The user
    /// of the API will be interfacing with this object, and not the CollisionDelegate object.
    /// </summary>
    public class CollisionDelegateReference
    {
        internal LinkedListNode<CollisionDelegate> m_Node;
        internal CollisionWorld.CollisionGroupIdentifier m_NodeGroup;
        internal CollisionWorld m_OwnerWorld;

        //  Prevents the user from instantiating a CollisionDelegateReference; only a CollisionWorld can create a CollisionDelegateReference.
        internal CollisionDelegateReference()
        {
        }

        /// <summary>
        /// Data contained within the delegate.
        /// </summary>
        public object data
        {
            get
            {
                return m_Node.Value.data;
            }

            set
            {
                m_Node.Value.data = value;
            }
        }

        /// <summary>
        /// Shape that the delegate uses to represent itself in the Collision World.
        /// </summary>
        public Shape shape
        {
            get
            {
                return m_Node.Value.shape;
            }

            set
            {
                m_Node.Value.shape = value;
            }
        }

        /// <summary>
        /// Whether or not the delegate is to be taken into account while determining collisions.
        /// </summary>
        public bool enabled
        {
            get
            {
                return m_Node.Value.enabled;
            }
            set
            {
                m_Node.Value.enabled = true;
            }
        }
    }

    /// <summary>
    /// Collection of grouped CollisionDelegates that is used for collision detection.
    /// </summary>
    public class CollisionWorld
    {
        private static CollisionWorld m_Instance = new CollisionWorld();
        public static CollisionWorld Instance
        {
            get
            {
                return m_Instance;
            }
        }

        private CollisionWorld()
        {

        }
        /// <summary>
        /// Set of identifiers used to group collision delegates for collision checks that only apply to a given group. Edit the members of this enum
        /// as is necessary.
        /// </summary>
        public enum CollisionGroupIdentifier
        {
            Faller,
            MouseClick
        }

        private Dictionary<CollisionGroupIdentifier, LinkedList<CollisionDelegate>> m_Delegates = new Dictionary<CollisionGroupIdentifier, LinkedList<CollisionDelegate>>();

        private void AutoCreateCollisionGroup(CollisionGroupIdentifier group)
        {
            if (!m_Delegates.ContainsKey(group))
            {
                m_Delegates.Add(group, new LinkedList<CollisionDelegate>());
            }
        }

        /// <summary>
        /// Adds the given delegate to the designated delegate group.
        /// </summary>
        /// <param name="group">Group the delegate should be added to.</param>
        /// <param name="collisionDelegate">Collision delegate to be added.</param>
        /// <returns>A CollisionDelegateReference to be used for removal of the delegate from the collision world and for manipulation of the delegate.</returns>
        public CollisionDelegateReference AddDelegate(CollisionGroupIdentifier group, CollisionDelegate collisionDelegate)
        {
            AutoCreateCollisionGroup(group);

            CollisionDelegateReference newReference = new CollisionDelegateReference();

            newReference.m_NodeGroup = group;
            newReference.m_Node = m_Delegates[group].AddLast(collisionDelegate);
            newReference.m_OwnerWorld = this;

            return newReference;
        }

        /// <summary>
        /// Removes the given delegate from the collision world.
        /// </summary>
        /// <param name="reference">Reference to the delegate contained by the collision world, returned by CollisionWorld::AddDelegate().</param>
        public void RemoveDelegate(CollisionDelegateReference reference)
        {
            m_Delegates[reference.m_NodeGroup].Remove(reference.m_Node);
        }

        public Vector2 DisallowIntersection(Shape shape, CollisionGroupIdentifier group, Vector2 Velocity, Shape.CollideType typeCheck = Shape.CollideType.All)
        {
            if (Velocity == Vector2.Zero)
                return shape.position;

            LinkedList<CollisionDelegate> delegates = m_Delegates[group];
            List<CollisionDelegate> CollidingDelegates = new List<CollisionDelegate>();

            Shape nextFrameShape = shape;
            Shape fixedPositionShape = shape;
            nextFrameShape.position += Velocity;



            Velocity.Normalize();

            foreach (CollisionDelegate cd in delegates)
            {
                while (nextFrameShape.Collides(cd.shape, typeCheck))
                {
                    if (!fixedPositionShape.Collides(cd.shape, typeCheck))
                    {
                        fixedPositionShape.position += Velocity;
                    }
                    else
                    {
                        fixedPositionShape.position -= Velocity;
                        nextFrameShape.position = fixedPositionShape.position;
                    }
                }
            }

            return nextFrameShape.position;
        }

        /// <summary>
        /// Returns whether or not an instance collides with the given shape.
        /// </summary>
        /// <param name="shape">Shape to be checked against for collision.</param>
        /// <param name="typeCheck">Definition of what a "collision" is for this check. Optional.</param>
        /// <returns></returns>
        public bool CollisionCheck(Shape shape, CollisionGroupIdentifier group, Shape.CollideType typeCheck = Shape.CollideType.All)
        {
            LinkedList<CollisionDelegate> delegates = m_Delegates[group];
            bool isColliding = false;

            foreach (CollisionDelegate cd in delegates)
            {
                if (shape.Collides(cd.shape, typeCheck))
                    isColliding = true;
            }
            return isColliding;
        }

        /// <summary>
        /// Returns the first instance that is detected to collide with the given shape.
        /// </summary>
        /// <param name="shape">Shape to be checked against for collision.</param>
        /// <param name="typeCheck">Definition of what a "collision" is for this check. Optional.</param>
        /// <returns></returns>
        public CollisionDelegate CollisionCheckFirstObject(Shape shape, CollisionGroupIdentifier group, Shape.CollideType typeCheck = Shape.CollideType.All)
        {
            LinkedList<CollisionDelegate> delegates = m_Delegates[group];
            CollisionDelegate firstInstance = new CollisionDelegate();
            foreach (CollisionDelegate cd in delegates)
            {
                if (shape.Collides(cd.shape, typeCheck))
                {
                    firstInstance = cd;
                    break;
                }
            }
            return firstInstance;
        }

        /// <summary>
        /// Returns a list of collision delegates containing all delegates whose shapes met the collision check with the given shape.
        /// </summary>
        /// <param name="shape">Shape to be compared against for collision.</param>
        /// <param name="typeCheck">Definition of what a "collision" is for this check. Optional.</param>
        /// <returns></returns>
        public List<CollisionDelegate> CollisionCheckAllObjects(Shape shape, CollisionGroupIdentifier group, Shape.CollideType typeCheck = Shape.CollideType.All)
        {
            LinkedList<CollisionDelegate> delegates = m_Delegates[group];
            List<CollisionDelegate> CollidingDelegates = new List<CollisionDelegate>();

            foreach (CollisionDelegate cd in delegates)
            {
                if (shape.Collides(cd.shape, typeCheck))
                {
                    CollidingDelegates.Add(cd);
                }
            }
            return CollidingDelegates;
        }
    }
}
