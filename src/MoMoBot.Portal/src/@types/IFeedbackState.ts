import ITwoDData from './ITwoDData';

export default interface IFeedbackState {
    feedbackStatistical?: ITwoDData[];
    popularIntents?: ITwoDData[];
    praiseFeedback?: ITwoDData[];
    badFeedback?: ITwoDData[];
}