using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class Function
{
    public float A { get; private set; } // constant terms
    public float B { get; private set; } // coefficient for x
    public float C { get; private set; } // coefficient for x^2

    public float D { get; private set; } // coefficient for x^3

    // TODO: might need refactor for when we support cubic function
    public Function (float a, float b, float c, float d = 0.0f)
    {
        A = a;
        B = b;
        C = c;
        D = d;
    }

    private bool isNonZero(float num)
    {
        return (num != 0.0f);
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder("y = ");
        bool isPrevious = false;
        if (isNonZero(A))
        {
            sb.Append($"{A}");
            isPrevious = true;
        }

        if (isNonZero(B))
        {
            if (isPrevious)
            {
                sb.Append(" + ");
            }
            sb.Append($"{B}x");
            isPrevious = true;
        }

        if (isNonZero(C))
        {
            if (isPrevious)
            {
                sb.Append(" + ");
            }
            sb.Append($"{C}x^2");
            isPrevious = true;
        }

        if (isNonZero(D))
        {
            if (isPrevious)
            {
                sb.Append(" + ");
            }
            sb.Append($"{D}x^3");
            isPrevious = true;
        }

        if (!isPrevious)
        {
            sb.Append("0");
        }

        return sb.ToString();
    }

    // Evaluate f(x) = a + bx + cx^2 + dx^3
    public float Evaluate(float x)
    {
        return (A + B * x + C * Mathf.Pow(x, 2) + D * Mathf.Pow(x, 3));
    }

    // Evaluate f'(x) = b + 2c*x + 3d*x^2
    public float FirstDerivative(float x)
    {
        return (B + 2 * C * x + 3 * D * Mathf.Pow(x, 2));
    }

    // Evaluate f'(x) = b + 2c*x + 3d*x^2
    public float SecondDerivative(float x)
    {
        return (2 * C + 6 * D * x);
    }
}
