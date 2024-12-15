package com.example.androidclient.requests

import com.example.androidclient.utils.Config
import io.ktor.client.call.body
import io.ktor.client.request.get
import io.ktor.client.request.post
import io.ktor.client.request.setBody
import kotlinx.serialization.Serializable

@Serializable
data class AccessToken (val accessToken: String)
@Serializable
data class RegistrationInfo (val email: String, val password: String)
@Serializable
data class AccountInfo (val uuid: String, val email: String)

suspend fun getAccessToken(context: Config, email: String, password: String): AccessToken? {
    createHttpClient(ApiRoutes.getAccountBaseUrl(context.apiHost)).use { client ->
        return try {
            client.get("${ApiRoutes.GET_ACCESS_TOKEN}?" +
                if (email.isEmpty()) "" else "email=$email&" +
                if (password.isEmpty()) "" else "password=$password").body()
        } catch (error: Exception) {
            error.printStackTrace(); null
        }
    }
}
suspend fun getAccountInfo(context: Config, accessToken: String): AccountInfo? {
    createHttpClient(ApiRoutes.getAccountBaseUrl(context.apiHost)).use { client ->
        return try {
            client.get("${ApiRoutes.GET_ACCOUNT_INFO}?" +
                if (accessToken.isEmpty()) "" else "accessToken=$accessToken"
            ).body()
        } catch (error: Exception) {
            error.printStackTrace(); null
        }
    }
}
suspend fun registrateAccount(context: Config, registrationInfo: RegistrationInfo): AccessToken? {
    createHttpClient(ApiRoutes.getAccountBaseUrl(context.apiHost)).use { client ->
        return try {
            client.post(ApiRoutes.REGISTRATION) { setBody(registrationInfo) }.body()
        } catch (error: Exception) {
            error.printStackTrace(); null
        }
    }
}
