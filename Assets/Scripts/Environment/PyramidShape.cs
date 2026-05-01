using UnityEngine;

namespace VoxelBeat.Environment
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class PyramidShape : MonoBehaviour
    {
        [Header("Pyramid Settings")]
        public float size = 0.4f;   // Ancho de la base
        public float length = 1.0f; // Largo hacia la punta (Eje Z)

        private void Awake()
        {
            GetComponent<MeshFilter>().mesh = GeneratePyramid();
        }

        private Mesh GeneratePyramid()
        {
            Mesh mesh = new Mesh();
            mesh.name = "Voxel_Pyramid";

            // 5 Vértices: 1 punta + 4 base
            Vector3[] vertices = new Vector3[]
            {
                new Vector3(0, 0, length),          // 0: Punta (Frente)
                new Vector3(-size, -size, 0),       // 1: Base Inf Izq
                new Vector3(size, -size, 0),        // 2: Base Inf Der
                new Vector3(size, size, 0),         // 3: Base Sup Der
                new Vector3(-size, size, 0)         // 4: Base Sup Izq
            };

            // Definir las caras (triángulos) en orden horario para que miren hacia afuera
            int[] triangles = new int[]
            {
                // Caras laterales
                0, 1, 2, // Cara Inferior
                0, 2, 3, // Cara Derecha
                0, 3, 4, // Cara Superior
                0, 4, 1, // Cara Izquierda

                // Base cuadrada (mirando hacia atrás)
                1, 4, 3,
                1, 3, 2
            };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}
