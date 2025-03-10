@file:OptIn(InternalAPI::class)

package com.example.androidclient.requests

import com.example.androidclient.utils.Config
import io.ktor.client.call.body
import io.ktor.client.request.forms.MultiPartFormDataContent
import io.ktor.client.request.forms.formData
import io.ktor.client.request.get
import io.ktor.client.request.header
import io.ktor.client.request.post
import io.ktor.client.utils.EmptyContent.headers
import io.ktor.http.ContentType
import io.ktor.http.Headers
import io.ktor.http.HttpHeaders
import io.ktor.http.HttpStatusCode
import io.ktor.http.append
import io.ktor.http.headers
import io.ktor.util.InternalAPI
import kotlinx.serialization.Serializable
import java.io.File

@Serializable
data class PostInfo(val title: String, val filePath: String, val uuid: String)

data class NewPost(val title: String, val imageData: ByteArray)

suspend fun getAllPosts(context: Config, accessToken: String): List<PostInfo>? {
    createHttpClient(ApiRoutes.getPostsBaseUrl(context.apiHost)).use { client ->
        return try {
            val response = client.get(ApiRoutes.GET_ALL_POSTS) {
                header(ApiAuth.AUTH_HEADER, ApiAuth.getAuthValue(accessToken))
            }
            response.body()
        } catch (error: Exception) {
           error.printStackTrace(); null
        }
    }
}
suspend fun addPost(context: Config, accessToken: String, newPost: NewPost): Boolean {
    createHttpClient(ApiRoutes.getPostsBaseUrl(context.apiHost)).use { client ->
        return try {
            val response = client.post(ApiRoutes.ADD_POST) {
                body = MultiPartFormDataContent(formData {
                    append("fileContent", newPost.imageData, Headers.build {
                        append(HttpHeaders.ContentType, ContentType.Image.PNG)
                        append(HttpHeaders.ContentDisposition, "filename=image.png")
                    })
                    append("title", newPost.title)
                })
                header(ApiAuth.AUTH_HEADER, ApiAuth.getAuthValue(accessToken))
            }
            response.status == HttpStatusCode.OK
        }
        catch (error: Exception) {
            error.printStackTrace(); false
        }
    }
}