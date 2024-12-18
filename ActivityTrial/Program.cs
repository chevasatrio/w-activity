using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

class Program
{
    static string connectionString = "server=localhost;database=jadwal_aktivitas;uid=root;pwd=;SslMode=none";
    static MySqlConnection connection = new MySqlConnection(connectionString);

    static void Main(string[] args)
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("================================================================");
            Console.WriteLine("-------------APLIKASI SEDERHANA MANAJEMEN AKTIVITAS-------------");
            Console.WriteLine("================================================================");
            Console.WriteLine("\nMenu Penjadwalan Aktivitas:");
            Console.WriteLine("1. Tambah Aktivitas");
            Console.WriteLine("2. Lihat Aktivitas");
            Console.WriteLine("3. Ubah Aktivitas");
            Console.WriteLine("4. Hapus Aktivitas");
            Console.WriteLine("5. Keluar");
            Console.Write("Pilih menu (1-5): ");

            try
            {
                int pilihan = int.Parse(Console.ReadLine());

                switch (pilihan)
                {
                    case 1:
                        TambahAktivitas();
                        break;
                    case 2:
                        LihatAktivitas();
                        break;
                    case 3:
                        UbahAktivitas();
                        break;
                    case 4:
                        HapusAktivitas();
                        break;
                    case 5:
                        running = false;
                        connection.Close();
                        break;
                    default:
                        Console.WriteLine("Pilihan tidak valid!");
                        break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Input tidak valid! Harap masukkan angka.");
            }
        }
    }

    // Tambah Aktivitas
    static void TambahAktivitas()
    {
        try
        {
            Console.Write("Masukkan nama tugas: ");
            string namaTugas = Console.ReadLine();

            Console.Write("Masukkan waktu mulai (yyyy-MM-dd HH:mm): ");
            DateTime waktuMulai = DateTime.Parse(Console.ReadLine());

            Console.Write("Masukkan waktu selesai (yyyy-MM-dd HH:mm): ");
            DateTime waktuSelesai = DateTime.Parse(Console.ReadLine());

            if (waktuSelesai <= waktuMulai)
            {
                throw new ArgumentException("Waktu selesai harus lebih besar dari waktu mulai!");
            }

            string query = "INSERT INTO aktivitas (nama_tugas, waktu_mulai, waktu_selesai) VALUES (@namaTugas, @waktuMulai, @waktuSelesai)";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                connection.Open();
                cmd.Parameters.AddWithValue("@namaTugas", namaTugas);
                cmd.Parameters.AddWithValue("@waktuMulai", waktuMulai);
                cmd.Parameters.AddWithValue("@waktuSelesai", waktuSelesai);
                cmd.ExecuteNonQuery();
                connection.Close();
            }

            Console.WriteLine("\nAktivitas berhasil ditambahkan!");
        }
        catch (FormatException)
        {
            Console.WriteLine("Format waktu salah! Harap masukkan dalam format yyyy-MM-dd HH:mm.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    // Lihat Aktivitas
    static void LihatAktivitas()
    {
        try
        {
            string query = "SELECT * FROM aktivitas";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                connection.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("Belum ada aktivitas yang dijadwalkan.");
                    }
                    else
                    {
                        Console.WriteLine("\nDaftar Aktivitas:");
                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader["id"]}, Tugas: {reader["nama_tugas"]}, " +
                                              $"Mulai: {reader["waktu_mulai"]}, Selesai: {reader["waktu_selesai"]}");
                        }
                    }
                }
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    // Ubah Aktivitas
    static void UbahAktivitas()
    {
        try
        {
            Console.Write("Masukkan ID aktivitas yang ingin diubah: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Masukkan nama tugas baru: ");
            string namaTugas = Console.ReadLine();

            Console.Write("Masukkan waktu mulai baru (yyyy-MM-dd HH:mm): ");
            DateTime waktuMulai = DateTime.Parse(Console.ReadLine());

            Console.Write("Masukkan waktu selesai baru (yyyy-MM-dd HH:mm): ");
            DateTime waktuSelesai = DateTime.Parse(Console.ReadLine());

            if (waktuSelesai <= waktuMulai)
            {
                throw new ArgumentException("Waktu selesai harus lebih besar dari waktu mulai!");
            }

            string query = "UPDATE aktivitas SET nama_tugas = @namaTugas, waktu_mulai = @waktuMulai, waktu_selesai = @waktuSelesai WHERE id = @id";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                connection.Open();
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@namaTugas", namaTugas);
                cmd.Parameters.AddWithValue("@waktuMulai", waktuMulai);
                cmd.Parameters.AddWithValue("@waktuSelesai", waktuSelesai);
                int rowsAffected = cmd.ExecuteNonQuery();
                connection.Close();

                if (rowsAffected == 0)
                {
                    throw new KeyNotFoundException("Aktivitas dengan ID tersebut tidak ditemukan!");
                }
            }

            Console.WriteLine("\nAktivitas berhasil diubah!");
        }
        catch (FormatException)
        {
            Console.WriteLine("Format waktu salah! Harap masukkan dalam format yyyy-MM-dd HH:mm.");
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    // Hapus Aktivitas
    static void HapusAktivitas()
    {
        try
        {
            Console.Write("Masukkan ID aktivitas yang ingin dihapus: ");
            int id = int.Parse(Console.ReadLine());

            string query = "DELETE FROM aktivitas WHERE id = @id";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                connection.Open();
                cmd.Parameters.AddWithValue("@id", id);
                int rowsAffected = cmd.ExecuteNonQuery();
                connection.Close();

                if (rowsAffected == 0)
                {
                    throw new KeyNotFoundException("Aktivitas dengan ID tersebut tidak ditemukan!");
                }
            }

            Console.WriteLine("\nAktivitas berhasil dihapus!");
        }
        catch (FormatException)
        {
            Console.WriteLine("Input tidak valid! Harap masukkan angka.");
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}