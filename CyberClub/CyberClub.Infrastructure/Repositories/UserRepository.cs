using CyberClub.Domain.Models;
using CyberClub.Domain.Interfaces;
using CyberClub.Infrastructure.DBService;
using Microsoft.Data.SqlClient;
using System.Data;
using CyberClub.Helper;
using CyberClub.Infrastructure.Interfaces;

namespace CyberClub.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly QueryBuilder _queryBuilder;

        public UserRepository(QueryBuilder queryBuilder)
        {
            _queryBuilder = queryBuilder;
        }

        public async Task<bool> AddUserAsync(User user)
        {
            string salt = SecurityHelper.GenerateSalt(70);
            string hashedPassword = SecurityHelper.HashPassword(user.HashPassword, salt, 10101, 70);

            string userQuery = @"
            INSERT INTO [User] ([Email], [FullName], [RoleId], [HashPassword], [Salt])
            OUTPUT INSERTED.[UserID]
            VALUES (@Email, @FullName, @RoleId, @HashPassword, @Salt)";

            SqlParameter[] userParams = {
            new SqlParameter("@Email", SqlDbType.NVarChar) { Value = user.Email },
            new SqlParameter("@FullName", SqlDbType.NVarChar) { Value = user.FullName },
            new SqlParameter("@RoleId", SqlDbType.Int) { Value = user.RoleId },
            new SqlParameter("@HashPassword", SqlDbType.NVarChar) { Value = hashedPassword },
            new SqlParameter("@Salt", SqlDbType.NVarChar) { Value = salt },
        };

            int userId = await _queryBuilder.ExecuteScalarAsync<int>(userQuery, userParams);

            if (userId > 0 && user.Profile != null)
            {
                string profileQuery = @"
                INSERT INTO [UserProfile] ([UserId], [PhoneNumber], [DOB])
                VALUES (@UserId, @PhoneNumber, @DOB)";

                SqlParameter[] profileParams = {
                new SqlParameter("@UserId", SqlDbType.Int) { Value = userId },
                new SqlParameter("@PhoneNumber", SqlDbType.NVarChar) { Value = (object?)user.Profile.PhoneNumber ?? DBNull.Value },
                new SqlParameter("@DOB", SqlDbType.Date) { Value = (object?)user.Profile.DOB ?? DBNull.Value }
            };

                int affectedRows = await _queryBuilder.ExecuteQueryAsync(profileQuery, profileParams);
                return affectedRows > 0;
            }

            return userId > 0;
        }

        public async Task<bool> AddUserProfileAsync(UserProfile profile)
        {
            string query = @"
        INSERT INTO UserProfile (UserId, PhoneNumber, DOB)
        VALUES (@UserId, @PhoneNumber, @DOB)";

            SqlParameter[] parameters = new SqlParameter[]
            {
            new SqlParameter("@UserId", profile.UserId),
            new SqlParameter("@PhoneNumber", profile.PhoneNumber),
            new SqlParameter("@DOB", profile.DOB)
            };

            int result = await _queryBuilder.ExecuteQueryAsync(query, parameters);
            return result > 0;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            string query = @"
        UPDATE [User]
        SET [Email] = @Email,
            [FullName] = @FullName,
            [RoleId] = @RoleId
        WHERE [Id] = @Id";

            SqlParameter[] parameters = {
        new SqlParameter("@Id", SqlDbType.Int) { Value = user.Id },
        new SqlParameter("@Email", SqlDbType.NVarChar) { Value = user.Email },
        new SqlParameter("@FullName", SqlDbType.NVarChar) { Value = user.FullName },
        new SqlParameter("@RoleId", SqlDbType.Int) { Value = user.RoleId }
    };

            int affectedRows = await _queryBuilder.ExecuteQueryAsync(query, parameters);
            return affectedRows > 0;
        }

        public async Task<bool> UpdateUserWithRoleAsync(User user, int newRoleId)
        {
            var query = @"
        UPDATE [User]
        SET [Email] = @Email,
            [FullName] = @FullName,
            [RoleId] = @RoleId
        WHERE [Id] = @Id";

            var parameters = new SqlParameter[]
            {
        new SqlParameter("@Id", SqlDbType.Int) { Value = user.Id },
        new SqlParameter("@Email", SqlDbType.NVarChar) { Value = user.Email },
        new SqlParameter("@FullName", SqlDbType.NVarChar) { Value = user.FullName },
        new SqlParameter("@RoleId", SqlDbType.Int) { Value = newRoleId }
            };

            int affectedRows = await _queryBuilder.ExecuteQueryAsync(query, parameters);
            return affectedRows > 0;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            User? user = null;
            string query = "SELECT [Id],[Email],[FullName],[RoleId] FROM [User] WHERE [Id] = @Id";
            await _queryBuilder.ExecuteQueryAsync(query, reader =>
            {
                if (reader.Read())
                {
                    user = new User
                    {
                        Id = reader.GetInt32(0),
                        Email = reader.GetString(1),
                        FullName = reader.GetString(2),
                        RoleId = reader.GetInt32(3)
                    };
                }
            }, new SqlParameter[] { new SqlParameter("@Id", SqlDbType.Int) { Value = id } });
            return user;
        }

        public async Task<bool> RegisterAsync(User user)
        {
            string salt = SecurityHelper.GenerateSalt(70);
            string hashedPassword = SecurityHelper.HashPassword(user.HashPassword, salt, 10101, 70);
            string query = @"
            INSERT INTO [User] (Email, HashPassword, Salt, FullName, RoleId)
            OUTPUT INSERTED.Id
            VALUES (@Email, @HashPassword, @Salt, @FullName, @RoleId)";

            SqlParameter[] parameters = {
                new SqlParameter("@Email", SqlDbType.VarChar) { Value = user.Email },
                new SqlParameter("@HashPassword", SqlDbType.VarChar) { Value = hashedPassword },
                new SqlParameter("@Salt", SqlDbType.VarChar) { Value = salt },
                new SqlParameter("@FullName", SqlDbType.VarChar) { Value = user.FullName },
                new SqlParameter("@RoleId", SqlDbType.Int) { Value = user.RoleId },
            };

            int userId = await _queryBuilder.ExecuteScalarAsync<int>(query, parameters);

            if (userId > 0)
            {
                string profileQuery = @"
    INSERT INTO [UserProfiles] ([UserId],[DOB]) VALUES (@UserId,@DOB)";
                SqlParameter[] profileParameters = {
    new SqlParameter("@UserId", SqlDbType.Int) { Value = userId },
    new SqlParameter("@DOB", SqlDbType.Date) { Value = user.Profile.DOB }
};


                int affectedRows = await _queryBuilder.ExecuteQueryAsync(profileQuery, profileParameters);
                return affectedRows > 0;
            }

            return false;
        }

        public async Task<bool> SignInAsync(User user)
        {
            string query = "SELECT [Salt] FROM [User] WHERE [Email] = @Email";
            string? salt = null;
            SqlParameter emailParam = new SqlParameter("@Email", SqlDbType.VarChar) { Value = user.Email };
            await _queryBuilder.ExecuteQueryAsync(query, reader =>
            {
                if (reader.Read())
                {
                    salt = reader.GetString(0);
                }
            }, new SqlParameter[] { emailParam });

            if (salt is null)
            {
                return false;
            }
            string hashedPassword = SecurityHelper.HashPassword(user.HashPassword, salt, 10101, 70);
            query = "SELECT COUNT(*) FROM [User] WHERE [Email] = @Email AND [HashPassword] = @HashPassword";
            emailParam = new SqlParameter("@Email", SqlDbType.VarChar) { Value = user.Email };
            SqlParameter hashPasswordParam = new SqlParameter("@HashPassword", SqlDbType.VarChar) { Value = hashedPassword };
            SqlParameter[] parameters = { emailParam, hashPasswordParam };
            int affectedRows = await _queryBuilder.ExecuteScalarAsync<int>(query, parameters);

            return affectedRows > 0;
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            User user = null;
            string query = "SELECT UserID, Email, FullName, HashPassword, Salt FROM [dbo].[User] WHERE Email = @Email";

            SqlParameter[] parameters = {
            new SqlParameter("@Email", email)
        };

            Action<SqlDataReader> readData = reader =>
            {
                if (reader.Read())
                {
                    user = new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("UserID")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        FullName = reader.GetString(reader.GetOrdinal("FullName")),
                        HashPassword = reader.GetString(reader.GetOrdinal("HashPassword")),
                        Salt = reader.GetString(reader.GetOrdinal("Salt"))
                    };
                }
            };

            await _queryBuilder.ExecuteQueryAsync(query, readData, parameters);

            return user;
        }
    }
}

