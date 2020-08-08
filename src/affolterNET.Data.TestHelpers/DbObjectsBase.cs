using System;
using System.Collections.Generic;
using FluentAssertions;

namespace affolterNET.Data.TestHelpers
{
    public abstract class DbObjectsBase
    {
        private readonly Dictionary<string, object> _dbobjects = new Dictionary<string, object>();

        public T Get<T>(string name) where T : class
        {
            return _dbobjects[name] as T;
        }

        protected T GetSet<T>(Func<T> create, string name)
            where T : class
        {
            if (name == null)
            {
                throw new InvalidOperationException("you have to give it a name");
            }

            if (!_dbobjects.ContainsKey(name))
            {
                var obj = create();

                // nochmals fragen - ein anderes create könnte das objekt schon erstellt haben
                if (!_dbobjects.ContainsKey(name))
                {
                    _dbobjects.Add(name, obj);
                }
            }

            var o = _dbobjects[name] as T;
            o.Should().NotBeNull("DbObject-Erstellen Fehler, Objekt ist null");
            return o;
        }
    }
}