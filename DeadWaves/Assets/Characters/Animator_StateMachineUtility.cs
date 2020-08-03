using System;
using UnityEngine;
/// <summary>
/// A <see cref="StateMachineBehaviour"/> with a context MonoBehaviour.
/// </summary>
/// <typeparam name="T">Type of the context</typeparam>
public class StateMachineBehaviour<T> : StateMachineBehaviour where T : Component
{
    /// <summary>
    /// The context MonoBehaviour owning the state machine.
    /// </summary>
    protected T Context { get; private set; }

    protected Animator Animator { get; private set; }

    /// <summary>
    /// The transform of the owning GameObject.
    /// </summary>
    protected Transform Transform { get; private set; }
    private bool _initialized;
    public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_initialized) {
            Animator = animator;
            Context = animator.GetComponentInParent<T>();
            if (Context == null)
                throw new InvalidOperationException(
                    $"State machine behaviour needs sibling/parent component of type {typeof(T)}");
            Transform = Context.transform;
            _initialized = true;
            OnInitialize(animator, stateInfo, layerIndex);
        }
        OnStateEntered(animator, stateInfo, layerIndex);
    }
    /// <summary>
    /// Initialize is called when the state is entered for the first time. Called before <see cref="OnStateEntered"/>
    /// </summary>
    protected virtual void OnInitialize(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
    /// <summary>
    /// Called every time the state is entered.
    /// </summary>
    protected virtual void OnStateEntered(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
}