using System;
using System.Configuration;
using System.Data;

namespace Kiss
{
    public class TransactionScope : IDisposable
    {
        private bool isCompleted = false;

        [ThreadStatic]
        private static IDbTransaction currentTransaction = null;

        public static IDbTransaction Transaction { get { return currentTransaction; } }

        public TransactionScope(ConnectionStringSettings css)
        {
            currentTransaction = Query.QueryFactory.Create(css.ProviderName).BeginTransaction(css.ConnectionString);
        }

        public void Complete()
        {
            isCompleted = true;
        }

        public void Dispose()
        {
            if (currentTransaction == null) return;

            try
            {
                if (isCompleted)
                    currentTransaction.Commit();
                else
                    currentTransaction.Rollback();
            }
            finally
            {
                IDbConnection conn = currentTransaction.Connection;
                if (conn != null && conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }

                currentTransaction.Dispose();

                currentTransaction = null;
            }
        }
    }
}
