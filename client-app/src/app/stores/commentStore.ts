import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { makeAutoObservable, runInAction } from "mobx";
import { ChatComment } from "../models/comment";
import { store } from "./store";

export default class CommentStore {
    comments: ChatComment[] = [];
    hubConnection: HubConnection | null = null;

    constructor()
    {
        // comments and hubconnection get marked as observables
        makeAutoObservable(this);
    }

    // create hub connection
    createHubConnection = (activityId: string) => {
        if(store.activityStore.selectedActivity)
        {
            this.hubConnection=new HubConnectionBuilder()
                .withUrl(process.env.REACT_APP_CHAT_URL + '?activityId=' + activityId, {
                    // we pass our token
                    accessTokenFactory: () => store.userStore.user?.token!
                })
                .withAutomaticReconnect()
                .configureLogging(LogLevel.Information)
                .build();

                this.hubConnection.start()
                    .catch(error=>console.log('Error establishing the connection: ', error));

                // this string must exactly same the one in the chathub.cs
                this.hubConnection.on('LoadComments', (comments: ChatComment[])=> {
                    // update our observable inside store
                    runInAction(()=> {
                        // converting string date value to javascript date value
                        comments.forEach(comment => {
                            // z for utc time
                            comment.createdAt=new Date(comment.createdAt + 'Z');
                        })
                        this.comments=comments
                    });
                })
                
                // get comment from server with 'ReceiveComment and push it to the server
                // users in the activity id signalr group will receive the comment
                this.hubConnection.on('ReceiveComment', (comment: ChatComment) => {
                    runInAction(() => {
                        // converting string date value to javascript date value
                        comment.createdAt=new Date(comment.createdAt);
                        //this.comments.push(comment)
                        this.comments.unshift(comment)
                    });
                })

        }

    }

    stopHubConnection = () => {
        this.hubConnection?.stop().catch(error => console.log('Error stopping connection: ', error));
    }

    clearComments = () => {
        this.comments = [];
        this.stopHubConnection();
    }

    addComment = async (values: any) => {
        values.activityId = store.activityStore.selectedActivity?.id;
        try {
            // we are calling the method on the backend
            // method name must be exactly same
            await this.hubConnection?.invoke('SendComment', values);

        } catch (error) {
            console.log(error);
        }
    }

}