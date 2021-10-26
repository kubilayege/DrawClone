using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public static class Helper
    {
        public static Vector3 GetWorldPos(Vector2 mousePoint, LayerMask layerMask = default)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePoint);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layerMask))
            {
	            if(hit.normal != Vector3.up)
		            return Vector3.positiveInfinity;
                return hit.point;
            }

            return Vector3.positiveInfinity;
        }
        
        public static bool TryGetObjectOfType<T>(Vector2 mousePoint, LayerMask layerMask, out T objectRef) where T : UnityEngine.Object
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePoint);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layerMask))
            {
                objectRef = hit.collider.gameObject.GetComponent<T>();

                return objectRef != null;
            }

            objectRef = null;
            return false;
        }
        
        public static bool TryOverlapOfType<T>(Vector2 mousePoint, LayerMask groundLayer, LayerMask targetLayer, out List<T> objectRefs) where T : UnityEngine.Object
        {
	        objectRefs = new List<T>();
	        Ray ray = Camera.main.ScreenPointToRay(mousePoint);
	        
	        if (!Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, groundLayer)) return false;
	        
	        var targets = Physics.OverlapSphere(hit.point, 3f, targetLayer);

	        if (targets == null) return objectRefs.Count != 0;
		        
	        foreach (var target in targets)
	        {
		        if (!target.TryGetComponent(out T objectRef)) continue;
				        
		        if(!objectRefs.Contains(objectRef))
		        {
			        objectRefs.Add(objectRef);
		        }
	        }


	        return objectRefs.Count != 0;

        }

        public static Vector3 Clamp(this Vector3 v, float size)
        {
            return Vector3.ClampMagnitude(v, size);
        }

        public static float AngleBetween(this Vector3 v, Vector3 other, Vector3 axis)
        {
            Vector3 vs = new Vector3(0, 0, 0);
            
            return Vector3.SignedAngle(v.normalized, other.normalized,axis);
        }

        public static int Remap(int value, int start1, int stop1, int start2, int stop2)
        {
            int outgoing =
                start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));

            return outgoing;
        }

        public static Vector3[] Add(this Vector3[] array, Vector3 element)
        {
            var newList = array.ToList();

            newList.Add(element);
            
            return newList.ToArray();
        }
        
        public static Vector3[] SmoothLine( Vector3[] inputPoints, float segmentSize )
        	{
        		//create curves
        		AnimationCurve curveX = new AnimationCurve();
        		AnimationCurve curveY = new AnimationCurve();
        		AnimationCurve curveZ = new AnimationCurve();
        
        		//create keyframe sets
        		Keyframe[] keysX = new Keyframe[inputPoints.Length];
        		Keyframe[] keysY = new Keyframe[inputPoints.Length];
        		Keyframe[] keysZ = new Keyframe[inputPoints.Length];
        
        		//set keyframes
        		for( int i = 0; i < inputPoints.Length; i++ )
        		{
        			keysX[i] = new Keyframe( i, inputPoints[i].x );
        			keysY[i] = new Keyframe( i, inputPoints[i].y );
        			keysZ[i] = new Keyframe( i, inputPoints[i].z );
        		}
        
        		//apply keyframes to curves
        		curveX.keys = keysX;
        		curveY.keys = keysY;
        		curveZ.keys = keysZ;
        
        		//smooth curve tangents
        		for( int i = 0; i < inputPoints.Length; i++ )
        		{
        			curveX.SmoothTangents( i, 0 );
        			curveY.SmoothTangents( i, 0 );
        			curveZ.SmoothTangents( i, 0 );
        		}
        
        		//list to write smoothed values to
        		List<Vector3> lineSegments = new List<Vector3>();
        
        		//find segments in each section
        		for( int i = 0; i < inputPoints.Length; i++ )
        		{
        			//add first point
        			lineSegments.Add( inputPoints[i] );
        
        			//make sure within range of array
        			if( i+1 < inputPoints.Length )
        			{
        				//find distance to next point
        				float distanceToNext = Vector3.Distance(inputPoints[i], inputPoints[i+1]);
        
        				//number of segments
        				int segments = (int)(distanceToNext / segmentSize);
        
        				//add segments
        				for( int s = 1; s < segments; s++ )
        				{
        					//interpolated time on curve
        					float time = ((float)s/(float)segments) + (float)i;
        
        					//sample curves to find smoothed position
        					Vector3 newSegment = new Vector3( curveX.Evaluate(time), curveY.Evaluate(time), curveZ.Evaluate(time) );
        
        					//add to list
        					lineSegments.Add( newSegment );
        				}
        			}
        		}
        
        		return lineSegments.ToArray();
        	}
    }
}