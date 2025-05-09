using UnityEngine;
using System;
using System.Reflection;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

[DisallowMultipleComponent]
public class RangeWeaponModifier : MonoBehaviour
{
    private RangeWeaponHandler targetHandler;
    private readonly BindingFlags _flags = BindingFlags.Instance | BindingFlags.NonPublic;//객체에 들어있으면서 public이 아닌 조건

    private void Start()
    {
        if (targetHandler == null)
            targetHandler = GetComponentInChildren<RangeWeaponHandler>();
        if (targetHandler == null)
        {
            Debug.LogError("[RangeWeaponModifier] 대상 Handler를 찾을 수 없습니다.");
            enabled = false;
            return;
        }
    }
    protected void ModifyField<T>(string fieldName, char op, T operand) where T : struct
    {
        var fi = typeof(RangeWeaponHandler)
            .GetField(fieldName, _flags);
        if (fi == null)
        {
            Debug.LogWarning($"[RangeWeaponModifier] '{fieldName}' 필드를 찾을 수 없습니다.");
            return;
        }

        var fieldType = fi.FieldType;
        if (fieldType == typeof(int))
        {
            int current = (int)fi.GetValue(targetHandler);
            int value = Convert.ToInt32(operand);
            int result;
            switch (op)
            {
                case '+': result = current + value; break;
                case '-': result = current - value; break;
                case '*': result = current * value; break;
                case '/':
                    result = value != 0 ? current / value
                                            : throw new DivideByZeroException();
                    break;
                default: return;
            }
            fi.SetValue(targetHandler, result);
        }
        else if (fieldType == typeof(float))
        {
            float current = (float)fi.GetValue(targetHandler);
            float value = Convert.ToSingle(operand);
            float result;
            switch (op)
            {
                case '+': result = current + value; break;
                case '-': result = current - value; break;
                case '*': result = current * value; break;
                case '/':
                    result = value != 0f ? current / value
                                              : throw new DivideByZeroException();
                    break;
                default: return;
            }
            fi.SetValue(targetHandler, result);
        }
        else
        {
            Debug.LogWarning($"[{nameof(RangeWeaponModifier)}] 지원하지 않는 타입: {fieldType}");
        }
    }
}
