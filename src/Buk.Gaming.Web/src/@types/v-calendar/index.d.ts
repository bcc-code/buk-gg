declare module 'v-calendar' {
    import Vue, { PluginFunction, PluginObject } from 'vue';

    class VCalendarPlugin implements PluginObject<{}> {
        [key: string]: any;
        public install: PluginFunction<{}>;
        Calendar: Calendar;
        DatePicker: DatePicker;
        Popover: Popover;
    }

    interface Calendar {}

    interface DatePicker {}

    interface Popover {}

    export function setupCalendar(userDefaults: any): any;

    export { Calendar, DatePicker, Popover };

    const VCalendar: VCalendarPlugin;
    export default VCalendar;
}
