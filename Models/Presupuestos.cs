namespace espacioPresupuestos;

using espacioPresupuestoDetalle;
using System.Collections.Generic;
using System;

public class Presupuesto
{
    public int idPresupuesto { get; set; }
    public string nombreDestinatario { get; set; }
    public DateTime fechaCreacion { get; set; }
    public List<PresupuestoDetalle> detalle { get; set; } = new List<PresupuestoDetalle>();

    // METODOS
    public float MontoPresupuesto()
    {
        float monto = 0f;
        if (detalle == null) return monto;
        foreach (var d in detalle)
        {
            if (d == null) continue;
            if (d.producto != null)
            {
                monto += d.producto.precio * d.cantidad;
            }
        }
        return monto;
    }

    public float MontoPresupuestoConIVA()
    {
        // IVA 21%
        float monto = MontoPresupuesto();
        return monto * 1.21f;
    }

    public int CantidadProductos()
    {
        int cantidad = 0;
        if (detalle == null) return cantidad;
        foreach (var d in detalle)
        {
            if (d == null) continue;
            cantidad += d.cantidad;
        }
        return cantidad;
    }
}