using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.2f; // alternatifi: public float moveSpeed = 0.2f;
    [SerializeField] private float runValue = 2;
    [SerializeField] private Animator animator;
    private SpriteRenderer targetRenderer;
    private float multiplier = 1;
    private bool isJumping;
    private Vector2 originalOffset, originalSize;
    private IEnumerator jumpResetCoroutine;
    

    private void Start() {
        // level 3
        targetRenderer = GetComponent<SpriteRenderer>();

        UIManager.instance.SetScore(DataManager.instance.score);

        originalOffset = GetComponent<CapsuleCollider2D>().offset;
        originalSize = GetComponent<CapsuleCollider2D>().size;
    }
    
    void Update()
    {
        float hor = Input.GetAxis("Horizontal") * Time.deltaTime; // A(-) ve D(+) tuslari
        // float ver = Input.GetAxis("Vertical") * Time.deltaTime; // S(-) ve W(+) tuslari
        float jump = Input.GetAxis("Jump");
        

        // level 3
        if (hor<0 && !targetRenderer.flipX) {
            targetRenderer.flipX=true;
        } else if (hor>0 && targetRenderer.flipX) {
            targetRenderer.flipX=false;
        }

        
        if (hor!=0f) {
            // shifte basiliysa
            if (Input.GetKey(KeyCode.LeftShift)) {
                multiplier = runValue;
                GetComponent<Animator>().SetFloat("moveMode", 2f);
            } else {
                // shifte basili degilse
                multiplier = 1;
                GetComponent<Animator>().SetFloat("moveMode", 1f);
            }
        } else 
            GetComponent<Animator>().SetFloat("moveMode", 0);

        
        if (jump>0 && !isJumping) {
            isJumping = true;
            GetComponent<Animator>().SetFloat("moveMode", 4f);
            
            GetComponent<CapsuleCollider2D>().offset = new Vector2(originalOffset.x, 1.68f);
            GetComponent<CapsuleCollider2D>().size = new Vector2(originalSize.x, 1.53f);
            GetComponent<Rigidbody2D>().simulated = false;
            
        }

        // CheckIfJumpAnimationIsEnding();



        transform.position += transform.right * (hor * moveSpeed * multiplier);
        CheckGround();
        
    }

    public void ResetColliderSizeAndOffset() {
        GetComponent<CapsuleCollider2D>().offset = originalOffset;
        GetComponent<CapsuleCollider2D>().size = originalSize;
        GetComponent<Rigidbody2D>().simulated = true;
        if (jumpResetCoroutine==null) {
            jumpResetCoroutine = Resetjump();
            StartCoroutine(jumpResetCoroutine);
        }
    }


    private IEnumerator Resetjump() {
        yield return new WaitForSeconds(0.2f);
        isJumping = false;
        jumpResetCoroutine = null;
    }


    private void CheckIfJumpAnimationIsEnding() {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("jump")) {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime>0.9f) {
                Debug.Log("jump is ending");
                ResetColliderSizeAndOffset();
            }
        }
    }


    // private void OnCollisionEnter2D(Collision2D other) {
    //     if(other.collider.tag=="Platform") {
    //         transform.SetParent(other.transform);
    //     }
    // }

    private void CheckGround() {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, 1f, 1<<31);
        if (hit && hit.transform.tag=="Platform") {
            transform.SetParent(hit.transform);
        } else {
            transform.SetParent(null);
        }
        // isJumping = false;
    }
}
