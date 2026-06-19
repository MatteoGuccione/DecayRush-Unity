using UnityEngine;
using UnityEngine.AI;
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;

    private static readonly int Speed = Animator.StringToHash("Speed");

    private Vector3 movementInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Raccoglie input da tastiera
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        movementInput = new Vector3(h, 0f, v).normalized;

        if (movementInput.magnitude > 0f)
        {
            animator.SetFloat(Speed, 1);
        }
        else
        {
            animator.SetFloat(Speed, 0);
        }

        // Applica movimento fisico
        Vector3 move = movementInput * moveSpeed;
        Vector3 newPosition = transform.position + move * Time.deltaTime;
        agent.Move(move * Time.deltaTime);
        //rb.MovePosition(newPosition);

        // Rotazione faccia verso la direzione
        if (movementInput.magnitude > 0f)
        {
            // Calcola la rotazione desiderata basandosi sull'input di movimento
            Quaternion targetRotation = Quaternion.LookRotation(movementInput);

            // Applica la rotazione in modo fluido usando Slerp
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

    }

    //void FixedUpdate()
    //{
    //    //Applica movimento fisico
    //   Vector3 move = movementInput * moveSpeed;
    //    Vector3 newPosition = transform.position + move * Time.fixedDeltaTime;
    //    agent.Move(move * Time.fixedDeltaTime);
    //    rb.MovePosition(newPosition);
    //    Debug.Log(newPosition);

    //    //Rotazione faccia verso la direzione
    //    if (movementInput != Vector3.zero)
    //    {

    //        Quaternion targetRotation = Quaternion.LookRotation(movementInput);
    //        rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

    //        //Calcola la rotazione desiderata basandosi sulla direzione della velocità
    //       Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);

    //        //Applica la rotazione in modo fluido usando Slerp(Spherical Linear Interpolation)
    //        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
    //    }


    //}
}
