using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Mis_Recordatorios.Models;

namespace Mis_Recordatorios.Controllers
{
    public class RecordatoriosController : Controller
    {
        private readonly string _connectionString;

        // Using your exact authenticated Supabase UUID
        private readonly Guid _usuarioId = Guid.Parse("e6b04e93-af28-48b9-bfb6-bc6048b66e5c");

        public RecordatoriosController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SupabaseConnection");
        }

        // ==========================================
        // ACTION 1: QUERY THE VIEW (READ)
        // ==========================================
        public IActionResult MisRecordatorios()
        {
            List<Recordatorio> lista = new List<Recordatorio>();

            using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT id, usuario_id, titulo, descripcion, fecha_recordatorio, completado FROM recordatorios WHERE usuario_id = @usuarioId ORDER BY fecha_recordatorio ASC";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("usuarioId", _usuarioId);

                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Recordatorio
                            {
                                Id = reader.GetGuid(0),
                                UsuarioId = reader.GetGuid(1),
                                Titulo = reader.GetString(2),
                                Descripcion = reader.IsDBNull(3) ? null : reader.GetString(3),
                                FechaRecordatorio = reader.GetDateTime(4),
                                Completado = reader.GetBoolean(5)
                            });
                        }
                    }
                }
            }

            return View(lista);
        }

        // ==========================================
        // ACTION 2: REGISTER NEW (SHOW VIEW)
        // ==========================================
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        // ==========================================
        // ACTION 3: REGISTER NEW (PROCESS FORM)
        // ==========================================
        [HttpPost]
        public IActionResult Crear(Recordatorio nuevo)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO recordatorios (usuario_id, titulo, descripcion, fecha_recordatorio, completado) VALUES (@usuarioId, @titulo, @descripcion, @fecha, FALSE)";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("usuarioId", _usuarioId);
                    cmd.Parameters.AddWithValue("titulo", nuevo.Titulo);
                    cmd.Parameters.AddWithValue("descripcion", (object)nuevo.Descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("fecha", nuevo.FechaRecordatorio);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("MisRecordatorios");
        }

        // ==========================================
        // ACTION 4: MARK AS COMPLETED
        // ==========================================
        [HttpPost]
        public IActionResult MarcarComoCompletado(Guid id)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE recordatorios SET completado = TRUE WHERE id = @id AND usuario_id = @usuarioId";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("usuarioId", _usuarioId);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("MisRecordatorios");
        }
    }
}