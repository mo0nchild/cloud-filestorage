package com.example.androidclient.requests
import io.ktor.client.HttpClient
import io.ktor.client.call.body
import io.ktor.client.call.receive
import io.ktor.client.engine.cio.CIO
import io.ktor.client.plugins.contentnegotiation.ContentNegotiation
import io.ktor.client.request.get
import io.ktor.client.request.header
import io.ktor.client.statement.HttpResponse
import io.ktor.client.statement.bodyAsText
import io.ktor.http.ContentType
import io.ktor.http.ContentType.Application.Json
import io.ktor.http.HttpHeaders
import io.ktor.serialization.kotlinx.json.json
import kotlinx.coroutines.*
import kotlinx.serialization.Serializable
import kotlinx.serialization.json.Json

private const val baseUrl = "http://192.168.0.103:5075"

@Serializable
data class TokenInfo (
    val uuid: String,
    val email: String
)

suspend fun sendGetRequest(): TokenInfo? {
    val client = HttpClient(CIO) {
        install(ContentNegotiation) {
            json(Json { ignoreUnknownKeys = true }) // Configure the JSON serializer to ignore unknown keys
        }
    }

    return try {
        client.get("$baseUrl/users/getTokens?Email=test@gmail.com&Password=1234567890") {
            header(HttpHeaders.Accept, ContentType.Application.Json)
        }
            .body<TokenInfo>()
    } catch (e: Exception) {
        e.printStackTrace()
        null
    } finally {
        client.close()
    }
}