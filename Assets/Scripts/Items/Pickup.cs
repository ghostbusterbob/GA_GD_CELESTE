using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private bool _pickedUp = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_pickedUp)
        {
            _pickedUp = true;
            _animator.SetBool("PickedUp", true);
            //FindAnyObjectByType<Score>().AddStrawberry();
        }
    }

    // Deze functie wordt door een Animation Event aangeroepen
    public void AnimationFinished()
    {   
        Destroy(gameObject);
    }
}
