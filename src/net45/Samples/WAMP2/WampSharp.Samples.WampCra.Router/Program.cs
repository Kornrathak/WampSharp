﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WampSharp.V2;
using WampSharp.V2.Authentication;
using WampSharp.V2.Realm;
using WampSharp.V2.Rpc;

namespace WampSharp.Samples.WampCra.Router
{
    public class Program
    {
        static void Main(string[] args)
        {
            HostCode();
            Console.ReadLine();
        }

        private static void HostCode()
        {
            DefaultWampAuthenticationHost host =
                new DefaultWampAuthenticationHost("ws://127.0.0.1:8080/ws",
                                                  new WampCraUserDbAuthenticationFactory(new MyAuthenticationProvider(),
                                                                                         new MyUserDb()));

            IWampHostedRealm realm = host.RealmContainer.GetRealmByName("realm1");

            string[] topics = new[]
            {
                "com.example.topic1",
                "com.foobar.topic1",
                "com.foobar.topic2"
            };

            foreach (string topic in topics)
            {
                string currentTopic = topic;

                realm.Services.GetSubject<string>(topic).Subscribe
                    (x => Console.WriteLine("event received on {0}: {1}", currentTopic, x));
            }

            realm.Services.RegisterCallee(new Add2Service()).Wait();

            host.Open();
        }

        public class Add2Service : IAdd2Service
        {
            public int Add2(int x, int y)
            {
                return (x + y);
            }
        }

        public interface IAdd2Service
        {
            [WampProcedure("com.example.add2")]
            int Add2(int a, int b);
        }

        private class MyAuthenticationProvider : WampCraStaticAuthenticationProvider
        {
            public MyAuthenticationProvider() :
                base(new Dictionary<string, WampCraAuthenticationRole>()
                {
                    {
                        "frontend",
                        new WampCraAuthenticationRole()
                        {
                            Permissions = new List<WampCraUriPermissions>()
                            {
                                new WampCraUriPermissions()
                                {
                                    Uri = "com.example.add2",
                                    CanCall = true
                                },
                                new WampCraUriPermissions()
                                {
                                    Uri = "com.example.",
                                    Prefixed = true,
                                    CanPublish = true
                                },
                                new WampCraUriPermissions()
                                {
                                    Uri = "com.example.topic2",
                                    CanPublish = false
                                },
                                new WampCraUriPermissions()
                                {
                                    Uri = "com.foobar.topic1",
                                    CanPublish = true
                                },
                            }
                        }
                    }
                })
            {
            }
        }

        private class MyUserDb : WampCraStaticUserDb
        {
            public MyUserDb() : base(new Dictionary<string, WampCraUser>()
                {
                    {
                        "joe", new WampCraUser()
                        {
                            Secret = "secret2",
                            AuthenticationRole = "frontend"
                        }
                    },
                    {
                        "peter", new WampCraUser()
                        {
                            Secret = "secret1",
                            AuthenticationRole = "frontend",
                            Salt = "salt123",
                            Iterations = 100,
                            KeyLength = 16
                        }
                    },
                })
            {
            }
        }
    }
}