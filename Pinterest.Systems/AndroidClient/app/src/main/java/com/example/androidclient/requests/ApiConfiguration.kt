package com.example.androidclient.requests

import io.ktor.client.HttpClient
import io.ktor.client.engine.android.Android
import io.ktor.client.plugins.DefaultRequest
import io.ktor.client.plugins.HttpTimeout
import io.ktor.client.plugins.contentnegotiation.ContentNegotiation
import io.ktor.client.plugins.defaultRequest
import io.ktor.client.plugins.logging.LogLevel
import io.ktor.client.plugins.logging.Logging
import io.ktor.client.request.accept
import io.ktor.client.request.header
import io.ktor.http.ContentType
import io.ktor.http.HttpHeaders
import io.ktor.http.URLProtocol
import io.ktor.serialization.kotlinx.json.json
import kotlinx.serialization.ExperimentalSerializationApi
import kotlinx.serialization.json.Json

const val SERVER_ADDRESS = "192.168.0.103"

object ApiRoutes {
    fun getAccountBaseUrl(host: String) = "$host:5102"
    fun getPostsBaseUrl(host: String) = "$host:5103"

    val GET_ACCESS_TOKEN = "/users/getTokens"
    val GET_ACCOUNT_INFO = "/users/getInfo"
    val REGISTRATION = "/users/registrate"

    val ADD_POST = "/posts/add"
    val GET_ALL_POSTS = "/posts/getAll"
}
object ApiAuth {
    const val AUTH_HEADER = "X-Authorization"
    fun getAuthValue(token: String) = "Bearer $token"
}

@OptIn(ExperimentalSerializationApi::class)
fun createHttpClient(baseUrl: String) = HttpClient(Android) {
    install(Logging) { level = LogLevel.ALL }
    defaultRequest {
        host = baseUrl
        url { protocol = URLProtocol.HTTP }
    }
    install(HttpTimeout) {
        requestTimeoutMillis = 15000L
        connectTimeoutMillis = 15000L
        socketTimeoutMillis = 15000L
    }
    install(ContentNegotiation) {
        json(
            Json {
                ignoreUnknownKeys = true
                prettyPrint = true
                isLenient = true
                explicitNulls = false
            }
        )
    }
    install(DefaultRequest) {
        header(HttpHeaders.ContentType, ContentType.Application.Json)
        accept(ContentType.Application.Json)
    }
}