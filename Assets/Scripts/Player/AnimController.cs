using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class AnimController : MonoBehaviour
{
	[SpineAnimation]
	public string IdleAnimationName;

	const int track = 1; // 해당 애니메이션 재생 트랙
	public SkeletonAnimation skeletonAnimation;

	IEnumerator Start()
	{
		if (skeletonAnimation == null) yield break;
		// 컴포넌트 할당 그리고 예외처리(방어적 프로그래밍 필수)
		while (true)
		{
			skeletonAnimation.AnimationState.SetAnimation(track, IdleAnimationName, false);
			// 해당 애니메이션 재생 순서대로 트랙, 재생할 애니메이션 클립, 반복여부

			yield return new WaitForSeconds(0.5f);
		}
	}
}


