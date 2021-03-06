﻿// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Writing.Formatting;

namespace ApacheJenaSample.Exporter.Async
{
    /// <summary>
    /// Simple Asynchronous Fuseki Client.
    /// </summary>
    public class AsyncFusekiConnector : IDisposable
    {
        private readonly SparqlFormatter formatter;
        private readonly HttpClient httpClient;
        private readonly string updateUri;

        public AsyncFusekiConnector(string updateUri)
        {
            this.updateUri = updateUri;
            this.formatter = new SparqlFormatter();
            this.httpClient = new HttpClient();
        }

        public async Task InsertTriplesAsync(string graph, IEnumerable<Triple> triples)
        {
            var query = GetSparqlQuery(graph, triples);

            var response = await httpClient
                .PostAsync(updateUri, new StringContent(query, Encoding.UTF8, "application/sparql-update"));

            response.EnsureSuccessStatusCode();
        }

        private string GetSparqlQuery(string graph, IEnumerable<Triple> additions)
        {
            var stringBuilder = new StringBuilder()
                .AppendLine("INSERT DATA {")
                .AppendLine("GRAPH <" + formatter.FormatUri(graph) + "> {");

            foreach (var addition in additions)
            {
                var formattedTriple = formatter.Format(addition);

                stringBuilder.AppendLine(formattedTriple);
            }

            return stringBuilder
                .AppendLine("}")
                .AppendLine("}")
                .ToString();
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}
