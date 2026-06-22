using System;

namespace Mis_Recordatorios.Models
{
    public class Recordatorio
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaRecordatorio { get; set; }
        public bool Completado { get; set; }

        // Esta es la propiedad clave que clasifica la "Situación" automáticamente
        public string Situacion
        {
            get
            {
                if (Completado)
                    return "Finalizado";

                // Usamos .Date para ignorar la hora y comparar solo los días
                if (FechaRecordatorio.Date < DateTime.Today)
                    return "Vencido";

                if (FechaRecordatorio.Date == DateTime.Today)
                    return "Hoy";

                return "Próximo";
            }
        }
    }
}