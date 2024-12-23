export interface ViewHandler {
    onCellRender(row: string, userLanguageCode: number): [string, string];
}
