import { template } from 'lodash';

/**
 * Compiles a string and interpolate data from the options argument.
 *
 * @param  {String} string
 * @param  {Object} options
 * @return {String}
 */
export function interpolate(input: string, options: any) {
    return template(input, { interpolate: /{{([\s\S]+?)}}/g })(options);
}

/**
 * Finds names of interpolation parameters in string
 *
 * @param  {String} string
 * @return {Array}
 */
export function getParameters(input: string) {
    const parameters = [];
    const re = /{{([\s\S]+?)}}/g;
    let match;
    // tslint:disable-next-line:no-conditional-assignment
    while ((match = re.exec(input)) !== null) {
        parameters.push(match[1]);
        match = re.exec(input);
    }
    return parameters;
}

export function camelCase(input: string) {
    if (input && input.length > 0) {
        return input.charAt(0).toLowerCase() + input.slice(1);
    } else {
        return input;
    }
}

export function capitalize(input: string) {
    if (input && input.length > 0) {
        return input.charAt(0).toUpperCase() + input.slice(1);
    } else {
        return input;
    }
}

export function isObject(val: any) {
    if (val === null) {
        return false;
    }
    return typeof val === 'function' || typeof val === 'object';
}
