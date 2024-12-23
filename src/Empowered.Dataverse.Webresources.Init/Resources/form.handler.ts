export interface FormHandler {
    formOnLoad(context: Xrm.Events.EventContext): void | Promise<void>;
}
