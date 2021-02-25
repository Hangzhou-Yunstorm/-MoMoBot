import IChatContact from './IChatContact';

export default interface IChatState {
    contacts?: IChatContact<number>[];
    activeRecord?: IChatContact<number>;
    unreadCount: number;
    records?: [];
    profile?: {},
    serviceRecords?: {}
}