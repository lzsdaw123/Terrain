using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GizmosExtension
{

    public static void DrawCircle(Vector3 position, Color color, float radius)
    {
        Vector3 _up = Vector3.up.normalized * radius;
        Vector3 _forward = Vector3.Slerp(_up, -_up, 0.5f);
        Vector3 _right = Vector3.Cross(_up, _forward).normalized * radius;

        Matrix4x4 matrix = new Matrix4x4();

        matrix[0] = _right.x;
        matrix[1] = _right.y;
        matrix[2] = _right.z;

        matrix[4] = _up.x;
        matrix[5] = _up.y;
        matrix[6] = _up.z;

        matrix[8] = _forward.x;
        matrix[9] = _forward.y;
        matrix[10] = _forward.z;

        Vector3 _lastPoint = position + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));
        Vector3 _nextPoint = Vector3.zero;

        Color oldColor = Gizmos.color;
        Gizmos.color = (color == default(Color)) ? Color.white : color;

        for (var i = 0; i < 91; i++)
        {
            _nextPoint.x = Mathf.Cos((i * 4) * Mathf.Deg2Rad);
            _nextPoint.z = Mathf.Sin((i * 4) * Mathf.Deg2Rad);
            _nextPoint.y = 0;

            _nextPoint = position + matrix.MultiplyPoint3x4(_nextPoint);

            Gizmos.DrawLine(_lastPoint, _nextPoint);
            _lastPoint = _nextPoint;
        }

        Gizmos.color = oldColor;
    }

    public static void DrawCircle(Vector3 position, float radius)
    {
        DrawCircle(position, Gizmos.color, radius);
    }

    public static void DrawCylinder(Vector3 position, float high, Color color, float radius)
    {
        Vector3 _up = Vector3.up.normalized * high * 0.5f;
        Vector3 _forward = Vector3.Slerp(Vector3.up.normalized * radius, -Vector3.up.normalized * radius, 0.5f);
        Vector3 _right = Vector3.Cross(Vector3.up.normalized * radius, _forward).normalized * radius;

        //Radial circles
        DrawCircle(position + _up, color, radius);
        DrawCircle(position - _up, color, radius);
        DrawCircle(position, color, radius);


        Color oldColor = Gizmos.color;
        Gizmos.color = color;

        //Side lines
        Gizmos.DrawLine(position + _right - _up, position + _right + _up);
        Gizmos.DrawLine(position - _right - _up, position - _right + _up);

        Gizmos.DrawLine(position + _forward - _up, position + _forward + _up);
        Gizmos.DrawLine(position - _forward - _up, position - _forward + _up);

        Gizmos.color = oldColor;
    }

    public static void DrawCylinder(Vector3 position, float high, float radius)
    {
        DrawCylinder(position, high, Gizmos.color, radius);
    }

    public static void DrawArc(Vector3 position, float radius, float angle, Color color, Quaternion rotation)
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = color;
        var old = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
        Vector3 from = Vector3.forward * radius;
        for (int i = 0; i <= angle; i++)
        {
            var to = new Vector3(radius * Mathf.Sin(i * Mathf.Deg2Rad), 0, radius * Mathf.Cos(i * Mathf.Deg2Rad));
            Gizmos.DrawLine(from, to);
            from = to;
        }

        Gizmos.matrix = old;
        Gizmos.color = oldColor;
    }

    public static void DrawArc(Vector3 position, float radius, float angle, Quaternion rotation)
    {
        DrawArc(position, radius, angle, Gizmos.color, rotation);
    }

    public static void DrawSector(Vector3 position, float radius, float angle, Color color, Quaternion rotation)
    {
        Color oldColor = Gizmos.color;
        DrawArc(position, radius, angle, color, rotation);
        // 畫夾角的線
        Quaternion rot = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
        Vector3 dline = rot * new Vector3(0, 0, radius) + position;
        Gizmos.DrawLine(position, dline);
        rot = Quaternion.Euler(0, rotation.eulerAngles.y+ angle, 0);
        dline = rot * new Vector3(0, 0, radius) + position;
        Gizmos.DrawLine(position, dline);

        Gizmos.color = oldColor;
    }

    public static void DrawSector(Vector3 position, float radius, float angle, Quaternion rotation)
    {
        DrawSector(position, radius, angle, Gizmos.color, rotation);
    }
}
