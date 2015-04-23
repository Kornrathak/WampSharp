﻿using WampSharp.V2.Binding;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.PubSub;
using WampSharp.V2.Rpc;
using WampSharp.V2.Session;

namespace WampSharp.V2.Realm
{
    public class WampRouterBuilder : IWampRouterBuilder
    {
        public IWampSessionServer<TMessage> CreateSessionHandler<TMessage>(IWampHostedRealmContainer realmContainer, IWampBinding<TMessage> binding, IWampEventSerializer eventSerializer)
        {
            return new WampSessionServer<TMessage>(binding, realmContainer, this, eventSerializer);
        }

        public virtual IWampBroker<TMessage> CreateBrokerHandler<TMessage>(IWampRealm realm, IWampBinding<TMessage> binding, IWampEventSerializer eventSerializer)
        {
            return new WampPubSubServer<TMessage>(realm.TopicContainer, eventSerializer, binding);
        }

        public IWampDealer<TMessage> CreateDealerHandler<TMessage>(IWampRealm realm, IWampBinding<TMessage> binding)
        {
            return new WampRpcServer<TMessage>(realm.RpcCatalog, binding);
        }
    }
}