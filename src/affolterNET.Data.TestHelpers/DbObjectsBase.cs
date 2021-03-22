﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace affolterNET.Data.TestHelpers
{
    public abstract class DbObjectsBase
    {
        private readonly ITestOutputHelper? _output;
        private readonly Dictionary<string, object> _dbobjects = new();

        public DbObjectsBase(ITestOutputHelper? output = null)
        {
            _output = output;
        }


        public void WriteLine(string msg)
        {
            if (_output == null)
            {
                Console.WriteLine(msg);
            }
            else
            {
                _output.WriteLine(msg);
            }
        }

        public void WriteLine(string msg, params object[] args)
        {
            if (_output == null)
            {
                Console.WriteLine(msg, args);
            }
            else
            {
                _output.WriteLine(msg, args);
            }
        }

        public T Get<T>(string name) where T : class
        {
            WriteLine($"Get: \"{name}\" ({typeof(T).FullName})");
            var obj = _dbobjects[name] as T;
            if (obj == null)
            {
                throw new InvalidOperationException("obj was null");
            }

            return obj;
        }

        public IEnumerable<T> GetAll<T>()
        {
            WriteLine($"GetAll: {typeof(T).FullName}");
            return _dbobjects.Values.Where(o => o.GetType() == typeof(T)).Cast<T>();
        }

        protected T Set<T>(Func<T> create, Func<T, string> getkey)
            where T : class
        {
            return Set(create, getkey, out _);
        }

        protected T Set<T>(Func<T> create, Func<T, string> getkey, out string key)
            where T : class
        {
            var obj = create(); 
            key = getkey(obj);
            _dbobjects.Add(key, obj);
            WriteLine($"set: " + JsonConvert.SerializeObject(obj));
            var o = _dbobjects[key] as T;
            o.Should().NotBeNull(
                $"Unter dem Namen \"{key}\" wurde bereits ein Objekt vom Typ \"{_dbobjects[key].GetType().FullName}\" hinzugefügt (aktuell: {typeof(T).FullName})");
            return o!;
        }

        [Obsolete("please use \"Set\"")]
        protected T GetSet<T>(Func<T> create, string name)
            where T : class
        {
            if (name == null) throw new InvalidOperationException("you have to give it a name");

            var added = false;
            if (!_dbobjects.ContainsKey(name))
            {
                var obj = create();

                WriteLine($"created: " + JsonConvert.SerializeObject(obj));

                // nochmals fragen - ein anderes create könnte das objekt schon erstellt haben
                if (!_dbobjects.ContainsKey(name))
                {
                    _dbobjects.Add(name, obj);
                    added = true;
                }
            }

            WriteLine($"\"{name}\" added: {added}");

            var o = _dbobjects[name] as T;
            o.Should().NotBeNull(
                $"Unter dem Namen \"{name}\" wurde bereits ein Objekt vom Typ \"{_dbobjects[name].GetType().FullName}\" hinzugefügt (aktuell: {typeof(T).FullName})");
            return o!;
        }
    }
}